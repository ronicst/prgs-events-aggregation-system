using System.Net;
using EventsTests.Model;
using RestSharp;
using EventsWebService.Dtos;
using EventsTests.Common;
using EventsTests.MessageManager;
using EventsTests.Configuration;

namespace EventsTests.MessageQueueTests
{
    [TestFixture]
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
        public void MqProductInstalledPositiveTest()
        {
            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductInstalled);
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
            Assert.That(receivedEvent?.Type, Is.EqualTo(MessageType.ProductInstalled));

            var productInstalled = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductInstalledDto>(receivedEvent?.Data.ToString());

            Assert.That(productInstalled?.ProductName, Is.EqualTo(productInstalledBody.ProductName));
            Assert.That(productInstalled.ProductVersion, Is.EqualTo(productInstalledBody.ProductVersion));
            Assert.That(productInstalled.Date, Is.EqualTo(productInstalledBody.Date));
            Assert.That(productInstalled.UserId, Is.EqualTo(productInstalledBody.UserId));
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            this.client.Dispose();
            this.mqConnection.Dispose();
        }
    }
}

