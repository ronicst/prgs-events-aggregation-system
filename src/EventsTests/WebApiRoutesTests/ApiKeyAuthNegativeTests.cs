using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EventsTests.Configuration;

namespace EventsTests.WebApiRoutesTests
{
    [TestFixture]
    internal class ApiKeyAuthNegativeTests
    {
        RestClient client = new RestClient(Constants.ApiUri);
        RestRequest? request;

        // If the API key is IsNullOrWhiteSpace, bad request

        [TestCase("Events", Method.Post)]
        public void TestApiKeyIsRequired(string resource, Method httpMethod)
        {
            this.request = new RestRequest(resource, httpMethod);

            // Add headers
            this.request.AddHeader("accept", Constants.Accept);
            this.request.AddHeader("Content-Type", Constants.ContentType);
            string apiKeyValue = "";
            this.request.AddHeader(Constants.ApiKeyHeaderName, apiKeyValue);
            // Send a request without API key in the header
            var response = client.Execute(request);

            // Verify that the response status code indicates Bad request
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void TestWithFakeApiKey()
        {
            this.request = new RestRequest(Constants.WebApiRouteEvents, Method.Post);

            // Add headers
            this.request.AddHeader("accept", Constants.Accept);
            this.request.AddHeader("Content-Type", Constants.ContentType);
            string apiKeyValue = "faa_api_key";
            this.request.AddHeader(Constants.ApiKeyHeaderName, apiKeyValue);
            // Send a request without API key in the header
            var response = client.Execute(request);

            // Verify that the response status code indicates Bad request
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void TestWithoutApiKey()
        {
            this.request = new RestRequest(Constants.WebApiRouteEvents, Method.Post);

            // Add headers
            this.request.AddHeader("accept", Constants.Accept);
            this.request.AddHeader("Content-Type", Constants.ContentType);
            // Send a request without API key in the header
            var response = client.Execute(request);

            // Verify that the response status code indicates Bad request
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            this.client.Dispose();
        }
    }
}
