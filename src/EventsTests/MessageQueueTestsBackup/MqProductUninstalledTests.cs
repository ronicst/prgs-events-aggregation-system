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
    internal class MqProductUninstalledTests
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
        public void MqProductUninstalledPositiveTests()
        {
            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), "ProductUninstalled");
            this.request.AddQueryParameter("type", eventName);

            // Build File Download body
            ProductUninstalledDto productUninstalledBody = new ProductUninstalledDto();
            productUninstalledBody.ProductName = "Test Product Name";
            productUninstalledBody.ProductVersion = "1.0";
            productUninstalledBody.Date = DateTime.Now;
            productUninstalledBody.UserId = Guid.NewGuid();

            var body = System.Text.Json.JsonSerializer.Serialize(productUninstalledBody);
            Console.WriteLine(body.ToString());
            this.request.AddBody(body);

            var response = client.ExecuteRequest(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var receivedEvent = this.mqConnection.ConsumeSingleEvent();

            Assert.That(receivedEvent, Is.Not.Null);
            Assert.That(receivedEvent?.Type, Is.EqualTo(MessageType.ProductUninstalled));

            var productUninstalled = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductInstalledDto>(receivedEvent?.Data.ToString());

            Assert.That(productUninstalled.ProductName, Is.EqualTo(productUninstalledBody.ProductName));
            Assert.That(productUninstalled.ProductVersion, Is.EqualTo(productUninstalledBody.ProductVersion));
            Assert.That(productUninstalled.Date, Is.EqualTo(productUninstalledBody.Date));
            Assert.That(productUninstalled.UserId, Is.EqualTo(productUninstalledBody.UserId));
        }

        // [TestCase("", "1.1", "11/12/2023", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd")] 
        // Bug: Request is successful with empty Product Name, uncomment when the bug is fixed.
        [TestCase("Product Name", "", "11/12/2023", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd")]
        [TestCase("Product Name", "1.2", "", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd")]
        [TestCase("Product Name", "1.3", "11/12/2023", "invalid_email")]
        [TestCase("Product Name", "1.4", "11/12/2023", "")]
        public void MqProductUninstalledInvalidDataTests(string productName, string productVersion, string date, string guid)
        {
            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), "ProductUninstalled");
            this.request.AddQueryParameter("type", eventName);

            // Build File Download body
            ProductUninstalledInvalidDto productUninstalledBody = new ProductUninstalledInvalidDto();
            productUninstalledBody.ProductName = productName;
            productUninstalledBody.ProductVersion = productVersion;
            productUninstalledBody.Date = date;
            productUninstalledBody.UserId = guid;

            var body = System.Text.Json.JsonSerializer.Serialize(productUninstalledBody);
            Console.WriteLine(body.ToString());
            this.request.AddBody(body);

            var response = client.ExecuteRequest(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var receivedEvent = this.mqConnection.ConsumeSingleEvent();
            Assert.That(receivedEvent, Is.Null);
        }

        // Play with invalid values for the body 

        [TearDown]
        public void Teardown()
        {
            this.client.Dispose();
            this.mqConnection.Dispose();
        }
    }
}

