using System.Threading.Channels;
using NUnit.Framework.Constraints;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http;
using System.Threading.Tasks;
using RestSharp;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace EventsTests.WebApiRoutesTests
{
    public class Tests
    {
        private IConnection connection;
        private IModel channel;

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Starting my tests...");
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 5672,
                RequestedHeartbeat = TimeSpan.FromSeconds(30)
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.BasicQos(prefetchSize: 0, prefetchCount: 5, global: false);
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                Console.WriteLine("Got message on test queue");
            };

            channel.BasicConsume(queue: "eventsQueue.tests",
                                 autoAck: true,
                                 consumer: consumer);



            // Create HttpClient instance
            HttpClient client = new HttpClient();
            {
                // Base URL of the API
                string baseUrl = "http://localhost:5083";

                // Endpoint you want to call
                string endpoint = "/environment";

                // Combine base URL and endpoint
                string url = baseUrl + endpoint;

                try
                {
                    // Make a GET request
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    // Check if the response is successful (status code in the range 200-299)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read response content as string
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(responseBody);
                    }
                    else
                    {
                        // If the response is not successful, print the status code
                        Console.WriteLine($"Failed with status code {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    // If an error occurs during the request, print the error message
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            connection.Dispose();
            channel.Dispose();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void EnvironmentPositive()
        {
            var client = new RestClient("http://localhost:5083");
            var request = new RestRequest("environment");
            var response = client.ExecuteGet(request);

            var data = JsonSerializer.Deserialize<JsonNode>(response.Content!)!;

            Console.WriteLine(data);
        }
    }
}