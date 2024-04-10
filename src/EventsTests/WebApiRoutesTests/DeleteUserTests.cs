using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EventsTests.Common;
using EventsTests.Model;
using RestSharp;
using EventsTests.Configuration;
using EventsTests.MessageManager;

namespace EventsTests.WebApiRoutesTests
{
    [TestFixture]
    internal class DeleteUserTests
    {
        EventsTestRestConnection client;

        [SetUp]
        public void Setup()
        {
            client = new EventsTestRestConnection();
            Console.WriteLine("On setup");
        }

        [TestCase]
        public void DeleteUserSuccessfully()
        {
            var request = client.GetApiRequest(Constants.WebApiRouteUsers, Method.Delete);

            // Add query parameter to the URL
            request.AddQueryParameter("userEmail", "test@test.com");
            var response = client.ExecuteRequest(request);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }


        [TestCase("invalid_email")] // Bug: No validation for the email format
        [TestCase(" ")]
        public void DeleteUserNegative(string email)
        {
            var request = client.GetApiRequest(Constants.WebApiRouteUsers, Method.Delete);
            // Add query parameter to the URL
            request.AddQueryParameter("userEmail", email);

            var response = client.ExecuteRequest(request);

            Assert.That(response.IsSuccessStatusCode, Is.False);            
        }

        // Play with invalid values for the body 

        [OneTimeTearDown]
        public void Teardown()
        {
            /**
             * This suite invokes the web API which in turn posts
             * multiple messages to the message queue, clear the queue
             * in order not to confuse the subsequent test suites.
             */
            RabbitMQManager.PurgeQueue();

            client.Dispose();
        }
    }
}
