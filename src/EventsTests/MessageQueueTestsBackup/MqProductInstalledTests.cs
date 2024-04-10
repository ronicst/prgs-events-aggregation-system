using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EventsTests.Model;
using RestSharp;
using System.Globalization;
using EventsWebService.Dtos;
using EventsTests.Common;
using EventsTests.MessageManager;
using System.Threading;
using Newtonsoft.Json;

namespace EventsTests.MessageQueueTests
{
    internal class MqProductInstalledTests
    {
        EventsTestRestConnection client;
        RestRequest request;
        RabbitMQManager mqConnection;

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
       
        [Test]
        public void MqProductInstalledPositiveTests()
        {
            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), "ProductInstalled");
            this.request.AddQueryParameter("type", eventName);

            // Build File Download body
            ProductInstalledDto productInstalledBody = new ProductInstalledDto();
            productInstalledBody.ProductName = "Test Product Name";
            productInstalledBody.ProductVersion = "1.0";
            productInstalledBody.Date = DateTime.Now;
            productInstalledBody.UserId = Guid.NewGuid();

            var body = System.Text.Json.JsonSerializer.Serialize(productInstalledBody);
            Console.WriteLine(body.ToString());
            this.request.AddBody(body);

            var response = client.ExecuteRequest(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var receivedEvent = this.mqConnection.ConsumeSingleEvent();
            Assert.That(receivedEvent, Is.Not.Null);

            var productInstalled = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductInstalledDto>(receivedEvent?.Data.ToString());

            Assert.That(receivedEvent, Is.Not.Null);
            Assert.That(receivedEvent?.Type, Is.EqualTo(MessageType.ProductInstalled));
            Assert.That(productInstalled?.ProductName, Is.EqualTo(productInstalledBody.ProductName));
            Assert.That(productInstalled.ProductVersion, Is.EqualTo(productInstalledBody.ProductVersion));
            Assert.That(productInstalled.Date, Is.EqualTo(productInstalledBody.Date));
            Assert.That(productInstalled.UserId, Is.EqualTo(productInstalledBody.UserId));
        }

        [TestCase("", "1.1", "11/12/2023", "test@test.com")]
        [TestCase("Producct Name", "", "11/12/2023", "test@test.com")]
        [TestCase("Producct Name", "1.1", "", "")]
        [TestCase("Producct Name", "1.1", "11/12/2023", "invalid_email")]
        [TestCase("Producct Name", "1.1", "13 Jan 2024", "test@test.com")]
        public void MqProductInstalledInvalidDataTests(string productName, string productVersion, string date, string email)
        {
            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), "ProductInstalled");
            this.request.AddQueryParameter("type", eventName);

            // Build File Download body
            ProductInstalledInvalidDto productInstalledBody = new ProductInstalledInvalidDto();
            productInstalledBody.ProductName = productName;
            productInstalledBody.ProductVersion = productVersion;
            productInstalledBody.Date = date;
            productInstalledBody.UserId = email;

            var body = System.Text.Json.JsonSerializer.Serialize(productInstalledBody);
            Console.WriteLine(body.ToString());
            this.request.AddBody(body);

            var response = client.ExecuteRequest(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var receivedEvent = this.mqConnection.ConsumeSingleEvent();

            Console.WriteLine("Check received msg: ...");
            //Console.WriteLine(receivedEvent.Data);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(receivedEvent, Is.Null.Or.Empty);
            Console.WriteLine("Received msg is null or empty, no messages in the queue ...");

        }


        [TearDown]
        public void Teardown()
        {
            this.client.Dispose();
            this.mqConnection.Dispose();
        }
    }
}

