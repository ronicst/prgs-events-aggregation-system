using EventsWebService.Dtos;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace EventsWebService.Infrastructure
{
    public class MessageSender
    {
        public string[] SendEvent(IEventDto payload, string eventType)
        {
            var validationresult = payload.Validate();
            if (payload.Validate().Length > 0)
            {
                return validationresult;
            }

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 5672
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                string message = JsonConvert.SerializeObject(new EventMessage { Type = eventType, Data = payload });
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = false;
                properties.ContentType = "application/json";

                channel.BasicPublish(exchange: "basicEvents",
                                     routingKey: "",
                                     basicProperties: properties,
                                     body: body);
            }

            return [];
        }

        public void SendUserDelete(string payload)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 5672
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                string message = JsonConvert.SerializeObject(new EventMessage { Type = "UserDelete", Data = payload });
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = false;
                properties.ContentType = "application/json";

                channel.BasicPublish(exchange: "basicEvents",
                                     routingKey: "",
                                     basicProperties: properties,
                                     body: body);
            }
        }
    }
}