using System.Net;
using EventsTests.Common;
using EventsTests.MessageManager;
using EventsTests.Model;
using RestSharp;
using EventsTests.Configuration;


namespace EventsTests.MessageQueueTests
{
    [TestFixture]
    internal class MqUserLogoutTests
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
        public void UserLogoutPositiveTest()
        {
            // Build File Download body
            UserLogoutDto userLogoutBody = new UserLogoutDto();
            userLogoutBody.Date = DateTime.Now;
            userLogoutBody.Email = "test@test.com";

            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogout);
            this.request.AddQueryParameter("type", eventValue);

            var body = System.Text.Json.JsonSerializer.Serialize(userLogoutBody);

            this.request.AddBody(body);
            var response = client.ExecuteRequest(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Console.WriteLine("Build conssumer to get message from the queue");
            var receivedEvent = this.mqConnection.ConsumeSingleEvent();

            Assert.That(receivedEvent, Is.Not.Null);
            Assert.That(receivedEvent.Type, Is.EqualTo(MessageType.UserLogout));

            var ur = Newtonsoft.Json.JsonConvert.DeserializeObject<UserLogoutDto>(receivedEvent.Data.ToString());
            Assert.That(ur?.Email, Is.EqualTo(userLogoutBody.Email));
            Assert.That(ur.Date, Is.EqualTo(userLogoutBody.Date));

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
