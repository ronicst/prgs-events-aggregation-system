using System.Net;
using EventsTests.Model;
using RestSharp;
using EventsTests.Common;
using EventsTests.MessageManager;
using EventsTests.Configuration;

namespace EventsTests.MessageQueueTests
{
    [TestFixture]
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
            request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);

            Console.WriteLine("Creating MQ manager");
            this.mqConnection = new RabbitMQManager();

            /**
            * This suite checks the requests sent by web API are received as messages,
            * clear the queue as setup step
            * in order not to confuse the subsequent tests.
            */
            RabbitMQManager.PurgeQueue();
            Console.WriteLine("Setup done");
        }
       
        [Test]
        public void MqProductUninstalledPositiveTest()
        {
            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductUninstalled);
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

            var productUninstalled = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductUninstalledDto>(receivedEvent?.Data.ToString());

            Assert.That(productUninstalled?.ProductName, Is.EqualTo(productUninstalledBody.ProductName));
            Assert.That(productUninstalled.ProductVersion, Is.EqualTo(productUninstalledBody.ProductVersion));
            Assert.That(productUninstalled.Date, Is.EqualTo(productUninstalledBody.Date));
            Assert.That(productUninstalled.UserId, Is.EqualTo(productUninstalledBody.UserId));
        }

        /*
        [TestCase("", "1.1", "11/12/2023", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd")] 
        // Bug: The request is successful, because of the empty product name is accepted
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
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var receivedEvent = this.mqConnection.ConsumeSingleEvent();

            Assert.That(receivedEvent, Is.Null);
        }
        */

        [OneTimeTearDown]
        public void Teardown()
        {
            this.client.Dispose();
            this.mqConnection.Dispose();
        }
    }
}

