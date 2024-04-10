using System;
using System.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventsTests.Common;
using EventsTests.Db;
using EventsTests.MessageManager;
using EventsTests.Model;
using Newtonsoft.Json;
using RestSharp;


namespace EventsTests.MessageQueueTests
{
    internal class MqFileDownloadTests
    {
        EventsTestRestConnection client;
        RestRequest request;
        RabbitMQManager mqConnection;
        

        [SetUp]
        public void Setup()
        {
            /*
            Configuration currentConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Configuration assemblyConfiguration = ConfigurationManager.OpenExeConfiguration(new Uri(GetType().Assembly.CodeBase).LocalPath);
            if (assemblyConfiguration.HasFile && string.Compare(assemblyConfiguration.FilePath, currentConfiguration.FilePath, true) != 0)
            {
                assemblyConfiguration.SaveAs(currentConfiguration.FilePath);
                //ConfigurationManager.RefreshSection("appSettings");
                //ConfigurationManager.RefreshSection("connectionStrings");
            }
            */

            Console.WriteLine("Starting Rest connection:");
            client = new EventsTestRestConnection();
            request = client.GetApiRequest("Events", Method.Post);

            Console.WriteLine("Creating MQ manager");
            this.mqConnection = new RabbitMQManager();

            Console.WriteLine("Setup done");
        }

        [Test]
        public void FileDownloadPositiveTest()
        {
            // var sc = new ServiceController("EventsProcessWindowsService");
            // sc.Stop();
            
            /*
            using (var db = new TestEventsContext())
            {
                var aaa = db.FileDownloads.Count();
                foreach (var x in db.FileDownloads)
                {
                    Console.WriteLine(x.ToString());
                }
                Console.WriteLine("Found {0} file download events before test", 
                                  aaa);
            }
            */
            
            // sc.Start();
            
            // Build File Download body
            FileDownloadDto fileDownloadBody = new FileDownloadDto();
            fileDownloadBody.Id = Guid.NewGuid();
            fileDownloadBody.Date = DateTime.Now;
            fileDownloadBody.FileName = "myfile.exe";
            Random rand = new Random();
            fileDownloadBody.FileLenght = rand.Next();

            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), "FileDownload");
            this.request.AddQueryParameter("type", eventValue);

            var body = System.Text.Json.JsonSerializer.Serialize(fileDownloadBody);

            this.request.AddBody(body);
            var response = client.ExecuteRequest(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));    
            Console.WriteLine("Test running in thread " + System.Threading.Thread.CurrentThread.Name);

            Console.WriteLine("Build conssumer to get message from the queue");

            var receivedEvent = this.mqConnection.ConsumeSingleEvent();

            Assert.That(receivedEvent, Is.Not.Null);
            Assert.That(receivedEvent.Type, Is.EqualTo(MessageType.FileDownload));

            var fd = Newtonsoft.Json.JsonConvert.DeserializeObject<FileDownloadDto>(receivedEvent.Data.ToString());
            Assert.That(fd.Date, Is.EqualTo(fileDownloadBody.Date));
            Assert.That(fd.Id, Is.EqualTo(fileDownloadBody.Id));
            Assert.That(fd.FileName, Is.EqualTo(fileDownloadBody.FileName));
            Assert.That(fd.FileLenght, Is.EqualTo(fileDownloadBody.FileLenght));

            Console.WriteLine("Message received from the queue");
        }

        [Test]
        public void FileDownloadInvalidDataTest()
        {
            // Build File Download body
            FileDownloadInvalidDto fileDownloadBody = new FileDownloadInvalidDto();
            fileDownloadBody.Id = Guid.NewGuid().ToString();
            fileDownloadBody.Date = "Test date";
            fileDownloadBody.FileName = "myfile.exe";
            Random rand = new Random();
            fileDownloadBody.FileLenght = rand.Next().ToString();

            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), "FileDownload");
            this.request.AddQueryParameter("type", eventValue);

            var body = System.Text.Json.JsonSerializer.Serialize(fileDownloadBody);

            this.request.AddBody(body);
            var response = client.ExecuteRequest(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        }

        [TearDown]
        public void Teardown()
        {
            this.client.Dispose();
            mqConnection.Dispose();
        }
    }
}
