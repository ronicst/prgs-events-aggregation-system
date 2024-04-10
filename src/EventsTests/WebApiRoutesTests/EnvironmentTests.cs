using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EventsTests.Common;
using RestSharp;
using EventsTests.Configuration;

namespace EventsTests.WebApiRoutesTests
{
    internal class EnvironmentTests
    {
        EventsTestRestConnection client;

        [SetUp]
        public void Setup()
        {
            client = new EventsTestRestConnection();
            
            Console.WriteLine("On setup");
        }

        [Test]
        public void EnvironmentPositive()
        {
            var request = client.GetApiRequest(Constants.WebApiRouteEnvironments, Method.Get);
            var response = client.ExecuteRequest(request);

            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public void EnvironmentGtmIdCheck()
        {
            var request = client.GetApiRequest(Constants.WebApiRouteEnvironments, Method.Get);
            var response = client.ExecuteRequest(request);

            var env = JsonSerializer.Deserialize<Model.Environment>(response.Content!)!;
            Assert.That(env.GtmId, Is.EqualTo("GTM-K5N5LX"));
        }

        [Test]
        // Bug: Mismatch in the supported events - InvoiceCreated is additionally listed
        public void EnvironmentSupportedEventsType()
        {
            // Get all enum values of SupportedEventType
            var enumValues = Enum.GetValues(typeof(Model.SupportedEventType));
            var expectedEvents = new List<string>();

            // Convert each enum value to its string representation and add it to the list
            foreach (Model.SupportedEventType enumValue in enumValues)
            {
                Console.WriteLine(enumValue.ToString());
                expectedEvents.Add(enumValue.ToString());
            }

            var request = client.GetApiRequest(Constants.WebApiRouteEnvironments, Method.Get);
            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.Content);

            var env = JsonSerializer.Deserialize<Model.Environment>(response.Content!)!;

            for (int i = 0; i < expectedEvents.Count; i++)
            {
                Assert.That(env.SupportedEvents[i], Is.EqualTo(expectedEvents[i]));
            }
        }

        [Test]
        public void EnvironmentNegativePost()
        {
            var request = client.GetApiRequest(Constants.WebApiRouteEnvironments, Method.Post);
            var response = client.ExecuteRequest(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }

        [Test]
        public void EnvironmentNegativePut()
        {
            var request = client.GetApiRequest(Constants.WebApiRouteEnvironments, Method.Put);
            var response = client.ExecuteRequest(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            client.Dispose();
        }
    }
}
