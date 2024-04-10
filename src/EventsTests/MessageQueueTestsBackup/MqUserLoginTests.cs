using System;
using System.Collections.Generic;
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
            request = client.GetApiRequest("Events", Method.Post);

            Console.WriteLine("Creating MQ manager");
            this.mqConnection = new RabbitMQManager();

            Console.WriteLine("Setup done");
        }
       
        [Test]
        public void MqUserLoginPositiveTests()
        {
            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), "UserLogin");
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
            var receivedEvent = this.mqConnection.ConsumeSingleEvent();

            Console.WriteLine(response.StatusDescription);
            Console.WriteLine(receivedEvent.Data);

            Assert.That(receivedEvent, Is.Not.Null);
            Assert.That(receivedEvent?.Type, Is.EqualTo(MessageType.UserLogin));

            var userLogin= Newtonsoft.Json.JsonConvert.DeserializeObject<UserLoginDto>(receivedEvent?.Data.ToString());

            Assert.That(userLogin?.Date, Is.EqualTo(userLoginBody.Date));
            Assert.That(userLogin.Email, Is.EqualTo(userLoginBody.Email));
            Assert.That(userLogin.UserId, Is.EqualTo(userLoginBody.UserId));
            Assert.That(userLogin.FirstName, Is.EqualTo(userLoginBody.FirstName));
            Assert.That(userLogin.LastName, Is.EqualTo(userLoginBody.LastName));
        }

        [TestCase("", "test@test.com", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "Tom", "Jerry")]
        [TestCase("11/11/2024", "", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "Tom", "Jerry")]
        [TestCase("11/11/2024", "test@test.com", "", "Tom", "Jerry")]
        [TestCase("11/11/2024", "test@test.com", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "", "Jerry")]
        [TestCase("11/11/2024", "test@test.com", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "Tom", "")]
        [TestCase("11/11/2024", "invalid-name", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "Tom", "Jerry")]
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

