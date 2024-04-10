using System;
using System.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventsTests.Common;
using EventsTests.Db;
using EventsTests.MessageManager;
using EventsTests.Model;
using Newtonsoft.Json;
using RestSharp;


namespace EventsTests.MessageQueueTests
{
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
            request = client.GetApiRequest("Events", Method.Post);

            Console.WriteLine("Creating MQ manager");
            this.mqConnection = new RabbitMQManager();

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
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), "UserRegistered");
            this.request.AddQueryParameter("type", eventValue);

            var body = System.Text.Json.JsonSerializer.Serialize(userRegisteredBody);

            this.request.AddBody(body);
            var response = client.ExecuteRequest(request);
            Console.WriteLine("Test running in thread " + System.Threading.Thread.CurrentThread.Name);

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

        [Test]
        public void UserRegisteredInvalidDataTest()
        {
            // Build File Download body
            UserRegisteredDto userRegisteredBody = new UserRegisteredDto();
            userRegisteredBody.Email = "test";
            userRegisteredBody.FirstName = "Tom";
            userRegisteredBody.LastName = "Jerry";
            userRegisteredBody.Company = "Test Inc";
            userRegisteredBody.Phone = "4234242345676";

            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), "UserRegistered");
            this.request.AddQueryParameter("type", eventValue);

            var body = System.Text.Json.JsonSerializer.Serialize(userRegisteredBody);

            this.request.AddBody(body);
            var response = client.ExecuteRequest(request);
            Console.WriteLine("Test running in thread " + System.Threading.Thread.CurrentThread.Name);

            Console.WriteLine("Build conssumer to get message from the queue");

            var receivedEvent = this.mqConnection.ConsumeSingleEvent();

            Assert.That(receivedEvent, Is.Null);

            Console.WriteLine("Message received from the queue");
        }

        [TearDown]
        public void Teardown()
        {
            this.client.Dispose();
            mqConnection.Dispose();
        }
    }
}
