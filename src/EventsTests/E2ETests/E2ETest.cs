using EventsTests.Common;
using EventsTests.Configuration;
using EventsTests.MessageManager;
using RestSharp;
using EventsTests.Model;

namespace EventsTests.E2ETests
{
    [TestFixture]
    internal class E2ETest
    {
        EventsTestRestConnection client;
        RestRequest request;
        RabbitMQManager mqConnection;
        WepApiRestRequests warr;

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
            this.warr = new WepApiRestRequests();

            Console.WriteLine("Setup done");
        }

        [Test]
        public void E2EFileDownloadTest() 
        {
            
            // Register user
            var respUserRegistered = this.warr.UserRegisteredStep();

            var receivedEvent = this.mqConnection.ConsumeSingleEvent();
            Assert.That(receivedEvent, Is.Not.Null);

            // User login
            var respUserLogin = this.warr.UserLoginStep();
            var userLogin = Newtonsoft.Json.JsonConvert.DeserializeObject<RestResponsesDto>(value: respUserLogin.Content);

            receivedEvent = this.mqConnection.ConsumeSingleEvent();
            Assert.That(receivedEvent, Is.Not.Null);

            // Execute event: File Download
            var respFileDownload = this.warr.FileDownloadStep();

            receivedEvent = this.mqConnection.ConsumeSingleEvent();
            Assert.That(receivedEvent, Is.Not.Null);

            // Logout
            var respUserLogout = this.warr.UserLogoutStep();

            receivedEvent = this.mqConnection.ConsumeSingleEvent();
            Assert.That(receivedEvent, Is.Not.Null);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            this.mqConnection.Dispose();
            this.client.Dispose();
        }
    }
}
