using System.Net;
using EventsTests.Common;
using EventsTests.MessageManager;
using EventsTests.Model;
using RestSharp;
using EventsTests.Configuration;


namespace EventsTests.MessageQueueTests
{
    [TestFixture]
    internal class MqUserRegisteredTests
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
        public void UserRegisteredPositiveTest()
        {
            // Build File Download body
            UserRegisteredDto userRegisteredBody = new UserRegisteredDto();
            userRegisteredBody.Email = "test@test.com";
            userRegisteredBody.FirstName = "Tom";
            userRegisteredBody.LastName = "Jerry";
            userRegisteredBody.Company = "Test Inc";
            userRegisteredBody.Phone = "4234242345676";

            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserRegistered);
            this.request.AddQueryParameter("type", eventValue);

            var body = System.Text.Json.JsonSerializer.Serialize(userRegisteredBody);

            this.request.AddBody(body);
            var response = client.ExecuteRequest(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Console.WriteLine("Build conssumer to get message from the queue");

            var receivedEvent = this.mqConnection.ConsumeSingleEvent();

            Assert.That(receivedEvent, Is.Not.Null);
            Assert.That(receivedEvent.Type, Is.EqualTo(MessageType.UserRegistered));

            var ur = Newtonsoft.Json.JsonConvert.DeserializeObject<UserRegisteredDto>(receivedEvent.Data.ToString());
            Assert.That(ur.Email, Is.EqualTo(userRegisteredBody.Email));
            Assert.That(ur.FirstName, Is.EqualTo(userRegisteredBody.FirstName));
            Assert.That(ur.LastName, Is.EqualTo(userRegisteredBody.LastName));
            Assert.That(ur.Company, Is.EqualTo(userRegisteredBody.Company));
            Assert.That(ur.Phone, Is.EqualTo(userRegisteredBody.Phone));

            Console.WriteLine("Message received from the queue");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            this.client.Dispose();
            mqConnection.Dispose();
        }
    }
}
