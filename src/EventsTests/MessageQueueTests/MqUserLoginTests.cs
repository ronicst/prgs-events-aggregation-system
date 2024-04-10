using System.Net;
using EventsTests.Model;
using RestSharp;
using EventsTests.Common;
using EventsTests.MessageManager;
using EventsTests.Configuration;

namespace EventsTests.MessageQueueTests
{
    [TestFixture]
    internal class MqUserLoginTests
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
        public void MqUserLoginPositiveTest()
        {
            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogin);
            this.request.AddQueryParameter("type", eventName);

            // Build File Download body
            UserLoginDto userLoginBody = new UserLoginDto();
            userLoginBody.Date = DateTime.Now;
            userLoginBody.Email = "test@test.com";
            userLoginBody.UserId = Guid.NewGuid();
            userLoginBody.FirstName = "John";
            userLoginBody.LastName = "Smith";

            var body = System.Text.Json.JsonSerializer.Serialize(userLoginBody);
            Console.WriteLine(body.ToString());
            this.request.AddBody(body);

            var response = client.ExecuteRequest(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));    

            var receivedEvent = this.mqConnection.ConsumeSingleEvent();

            Assert.That(receivedEvent, Is.Not.Null);
            Assert.That(receivedEvent?.Type, Is.EqualTo(MessageType.UserLogin));

            var userLogin= Newtonsoft.Json.JsonConvert.DeserializeObject<UserLoginDto>(receivedEvent?.Data.ToString());

            Assert.That(userLogin?.Date, Is.EqualTo(userLoginBody.Date));
            Assert.That(userLogin.Email, Is.EqualTo(userLoginBody.Email));
            Assert.That(userLogin.UserId, Is.EqualTo(userLoginBody.UserId));
            Assert.That(userLogin.FirstName, Is.EqualTo(userLoginBody.FirstName));
            Assert.That(userLogin.LastName, Is.EqualTo(userLoginBody.LastName));
        }
        /*
        [TestCase("11/11/2024", "test@test.com", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "", "Jerry")] // Bug: Request accept empty FirstName
        [TestCase("11/11/2024", "test@test.com", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "Tom", "")] // Bug: Request accept empty LastName
        public void MqUserLoginInvalidDataTests(string date, string email, string guid, string firstName, string lastName)
        {
            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), "UserLogin");
            this.request.AddQueryParameter("type", eventName);

            // Build File Download body
            UserLoginInvalidDto userLoginBody = new UserLoginInvalidDto();
            userLoginBody.Date = date;
            userLoginBody.Email = email;
            userLoginBody.UserId = guid;
            userLoginBody.FirstName = firstName;
            userLoginBody.LastName = lastName;

            var body = System.Text.Json.JsonSerializer.Serialize(userLoginBody);
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

