using EventsTests.Model;
using NUnit.Framework.Internal.Execution;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EventsTests.MessageManager
{
    internal class RabbitMQManagerCleanup : IDisposable
    {
        private IConnection? connection;
        private IModel? channel;
        private EventingBasicConsumer? messageConsumer;
        private List<EventMessageDto> receivedEvents = new List<EventMessageDto>();
        readonly object sync = new object();

        public void MQManagerCleanup()
        {
            string queueName = "eventsQueue.tests";

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

            bool autoAck = true;

            // Retrieve a message from the queue
            BasicGetResult? result = channel.BasicGet(queueName, autoAck);
            if (result == null)
            {
                Console.WriteLine("=== No message available at this time. ===");
            }
            else
            {
                // Get the message properties and body
                IBasicProperties props = result.BasicProperties;
                byte[] body = result.Body.ToArray();

                // Convert the message body to a string
                string messageBody = System.Text.Encoding.UTF8.GetString(body);

                // Display the message content
                Console.WriteLine("Received message:");
                Console.WriteLine("Message body: " + messageBody);

                // Acknowledge the message
                channel.BasicAck(result.DeliveryTag, false);
            }
        }
        public void Dispose()
        {
            this.channel.Dispose();
            this.connection.Dispose();
        }
    }
}
