using EventsProcessWindowsService.Contracts;
using EventsProcessWindowsService.Db;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace EventsProcessWindowsService
{
    public partial class EventsProcessorService : ServiceBase
    {
        private IConnection connection;
        private IModel channel;

        public EventsProcessorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 5672,
                RequestedHeartbeat = TimeSpan.FromSeconds(30)
            };

            this.connection = factory.CreateConnection();
            this.channel = this.connection.CreateModel();

            this.channel.BasicQos(prefetchSize: 0, prefetchCount: 5, global: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                string messageBody = Encoding.UTF8.GetString(ea.Body.ToArray());
                var receivedEvent = JsonConvert.DeserializeObject<EventMessageDto>(messageBody);

                switch (receivedEvent.Type)
                {
                    case MessageType.FileDownload:
                        var fileDownloadEvent = JsonConvert.DeserializeObject<FileDownloadDto>(receivedEvent.Data.ToString());
                        using (EventsContext db = new EventsContext())
                        {
                            db.FileDownloads.Add(new Db.DataObjects.FileDownloadEvent 
                                { 
                                    EventId = fileDownloadEvent.Id.ToString(), 
                                    Date = fileDownloadEvent.Date.ToLongDateString(), 
                                    FileLenght = fileDownloadEvent.FileLenght, 
                                    FileName = fileDownloadEvent.FileName
                            });
                            db.SaveChanges();
                        }
                        break;
                    case MessageType.UserLogin:
                        var userLoginEvent = JsonConvert.DeserializeObject<UserLoginDto>(receivedEvent.Data.ToString());
                        using (EventsContext db = new EventsContext())
                        {
                            db.UserLogins.Add(new Db.DataObjects.UserLoginEvent
                            {
                                Date = userLoginEvent.Date.ToLongDateString(),
                                Email = userLoginEvent.Email,
                                UserId = userLoginEvent.UserId.ToString()
                            });
                            db.SaveChanges();
                        }
                        break;
                    case MessageType.UserDelete:
                        using (EventsContext db = new EventsContext())
                        {
                            var userData = db.UserLogins.Where(x => x.Email.Equals(receivedEvent.Data.ToString(), StringComparison.InvariantCultureIgnoreCase));
                            db.UserLogins.RemoveRange(userData);
                            var user = db.Users.FirstOrDefault(x => x.UserEmail == receivedEvent.Data.ToString());
                            if (user != null)
                            {
                                db.Users.Remove(user);
                            }
                            db.SaveChanges();
                        }
                        break;
                    case MessageType.UserRegistered:
                        using (EventsContext db = new EventsContext())
                        {
                            var userRegistered = JsonConvert.DeserializeObject<UserRegisteredDto>(receivedEvent.Data.ToString());
                            var user = db.Users.FirstOrDefault(x => x.UserEmail.Equals(userRegistered.Email, StringComparison.InvariantCultureIgnoreCase));
                            if (user == null)
                            {
                                db.Users.Add(new Db.DataObjects.User
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    UserName = $"{userRegistered.FirstName} {userRegistered.LastName}",
                                    UserEmail = userRegistered.Email,
                                    UserCompanyName = userRegistered.Company,
                                    DateRegistered = DateTime.UtcNow.ToString()
                                });
                                db.SaveChanges();
                            }
                        }
                        break;
                    case MessageType.ProductInstalled:
                        var productInstalledEvent = JsonConvert.DeserializeObject<ProductInstalledDto>(receivedEvent.Data.ToString());
                        using (EventsContext db = new EventsContext())
                        {
                            db.ProductActions.Add(new Db.DataObjects.ProductActionTraking
                            {
                                Date = productInstalledEvent.Date.ToShortDateString(),
                                UserId = productInstalledEvent.UserId.ToString(),
                                ProductName = productInstalledEvent.ProductName,
                                ProductVersion = productInstalledEvent.ProductVersion,
                                ActionType = "Instalation"
                            });
                            db.SaveChanges();
                        }
                        break;
                    case MessageType.ProductUninstalled:
                        var productUninstalledEvent = JsonConvert.DeserializeObject<ProductUninstalledDto>(receivedEvent.Data.ToString());
                        using (EventsContext db = new EventsContext())
                        {
                            db.ProductActions.Add(new Db.DataObjects.ProductActionTraking
                            {
                                Date = productUninstalledEvent.Date.ToShortDateString(),
                                UserId = productUninstalledEvent.UserId.ToString(),
                                ProductName = productUninstalledEvent.ProductName,
                                ProductVersion = productUninstalledEvent.ProductVersion,
                                ActionType = "Uninstalation"
                            });
                            db.SaveChanges();
                        }
                        break;
                    case MessageType.UserLogout:
                        var userLogoutEvent = JsonConvert.DeserializeObject<UserLoginDto>(receivedEvent.Data.ToString());
                        using (EventsContext db = new EventsContext())
                        {
                            db.UserLogouts.Add(new Db.DataObjects.UserLogOutEvent
                            {
                                LogoutTime = userLogoutEvent.Date.ToUniversalTime().ToString(),
                                Email = userLogoutEvent.Email
                            });
                            db.SaveChanges();
                        }
                        break;
                    default:
                        break;
                }
            };

            channel.BasicConsume(queue: "eventsQueue",
                                 autoAck: true,
                                 consumer: consumer);
        }

        protected override void OnStop()
        {
            this.channel.Dispose();
            this.connection.Dispose();
        }
    }
}