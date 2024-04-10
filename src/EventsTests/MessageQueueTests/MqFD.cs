using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventsTests.Common;
using EventsTests.MessageManager;
using EventsTests.Model;
using Newtonsoft.Json;
using NUnit.Framework.Internal.Execution;
using RabbitMQ.Client;
using RestSharp;

namespace EventsTests
{
    internal class MqFD
    {
        EventsTestRestConnection client;
        RestRequest request;
        RabbitMQManager mqConnection;
        bool mqMessageReceived = false;

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Starting Rest connection:");
            client = new EventsTestRestConnection();
            request = client.GetApiRequest("Events", Method.Post);

            Console.WriteLine("Creating MQ manager");
            this.mqConnection = new RabbitMQManager();

            Console.WriteLine("Setup done");
        }

        public string EventsPositive()
        {

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
            // End of POST request

            Console.WriteLine("Build conssumer to get message from the queue");
            // Create a manual reset event
            var manualResetEvent = new ManualResetEvent(false);

            // Build conssumer to get message from the queue
            // Connect to RabbitMQ server

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 5672,
                RequestedHeartbeat = TimeSpan.FromSeconds(30)
            };

            // this.connection = factory.CreateConnection();
            // this.channel = this.connection.CreateModel();

            // this.channel.BasicQos(prefetchSize: 0, prefetchCount: 5, global: false);

            using (var connection = factory.CreateConnection())
            {
                // Create a channel
                using (var channel = connection.CreateModel())
                {
                    // Declare the queue
                    channel.QueueDeclare(queue: "eventsQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    // Setup a consumer to receive messages from the queue
                    var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(channel);
                    string? receivedMessage = null;

                    // Event handler for receiving messages
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        receivedMessage = Encoding.UTF8.GetString(body);
                        Console.WriteLine("Received message from queue: " + receivedMessage);
                    };

                    // Start consuming messages from the queue
                    channel.BasicConsume(queue: "eventsQueue.tests", autoAck: true, consumer: consumer);

                    // Wait for a message to be received
                    while (receivedMessage == null)
                    {
                        Thread.Sleep(100); // Wait for 100 milliseconds before checking again
                    }

                    return receivedMessage;
                }
            }

        }
            [Test]
            public void mqTests()
            {
                string msg = EventsPositive();
            Console.WriteLine(msg);

            }

            [OneTimeTearDown]
            public void Teardown()
            {
                this.client.Dispose();
            }
        
    }
}
