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
using EventsTests.Configuration;

namespace EventsTests.WebApiRoutesTests
{
    internal class ProductUninstalledTests
    {
        EventsTestRestConnection client;
        RestRequest request;

        [SetUp]
        public void Setup()
        {
            client = new EventsTestRestConnection();
            Console.WriteLine("On setup");
            request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);
        }

        [Test]
        public void ProductUninstalledTestsPositive()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductUninstalled);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            ProductUninstalledDto productUninstalledBody = new ProductUninstalledDto();
            productUninstalledBody.ProductName = "Test Product Name";
            productUninstalledBody.ProductVersion = "1.0";
            productUninstalledBody.Date = DateTime.Now;
            productUninstalledBody.UserId = Guid.NewGuid();

            var body = JsonSerializer.Serialize(productUninstalledBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        // Bug: Error is not handled, the error message is "Unexpected error"
        public void ProductUninstalledInvalidDateTests()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductUninstalled);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            ProductUninstalledInvalidDto productUninstalledBody = new ProductUninstalledInvalidDto();
            productUninstalledBody.ProductName = "Test Product Name";
            productUninstalledBody.ProductVersion = "1.0";
            productUninstalledBody.Date = "Fake-date";
            productUninstalledBody.UserId = Guid.NewGuid().ToString();

            var body = JsonSerializer.Serialize(productUninstalledBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Invalid date format"));
        }

        [Test]
        // Bug: Error is not handled, the error message is "Unexpected error"
        public void ProductUninstalledInvalidGuidTests()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductUninstalled);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            ProductUninstalledInvalidDto productUninstalledBody = new ProductUninstalledInvalidDto();
            productUninstalledBody.ProductName = "Test Product Name";
            productUninstalledBody.ProductVersion = "1.0";
            productUninstalledBody.Date = DateTime.Now.ToString();
            productUninstalledBody.UserId = "fake-GUID";

            var body = JsonSerializer.Serialize(productUninstalledBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Invalid GUID format"));
        }

        [Test]
        // Bug: No field size for product name
        public void ProductUninstalledInvalidLongProductNameTests()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductUninstalled);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            ProductUninstalledDto productUninstalledBody = new ProductUninstalledDto();
            GenerateString generateString = new GenerateString();
            productUninstalledBody.ProductName = generateString.createString(10000, 'a');
            productUninstalledBody.ProductVersion = "1.0";
            productUninstalledBody.Date = DateTime.Now;
            productUninstalledBody.UserId = Guid.NewGuid();

            var body = JsonSerializer.Serialize(productUninstalledBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.Content, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Invalid ProductName format"));

        }

        [Test]
        // Bug: No field size restrictions for product version
        public void ProductUninstalledInvalidLongProductVersionTests()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductUninstalled);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            ProductUninstalledDto productUninstalledBody = new ProductUninstalledDto();
            productUninstalledBody.ProductName = "Test Product Name";
            GenerateString generateString = new GenerateString();
            productUninstalledBody.ProductVersion = generateString.createString(10000, '1');
            productUninstalledBody.Date = DateTime.Now;
            productUninstalledBody.UserId = Guid.NewGuid();

            var body = JsonSerializer.Serialize(productUninstalledBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.Content, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Invalid ProductVersion format"));
        }

        [TestCase("Test Product Name", "1.1", "12/12/2023", "")]
        public void ProductUninstalledRequiredEmptyGuidTests(string productName, string productVersion, string date, string guid)
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductUninstalled);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            ProductUninstalledInvalidDto productUninstalledBody = new ProductUninstalledInvalidDto();
            productUninstalledBody.ProductName = productName;
            productUninstalledBody.ProductVersion = productVersion;
            productUninstalledBody.Date = date;
            productUninstalledBody.UserId = guid;

            var body = JsonSerializer.Serialize(productUninstalledBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Invalid GUID format"));
        }

        [TestCase("Product Name", "1.1", "", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd")]
        public void ProductUninstalledRequiredEmptyDateTests(string productName, string productVersion, string date, string guid)
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductUninstalled);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            ProductUninstalledInvalidDto productUninstalledBody = new ProductUninstalledInvalidDto();
            productUninstalledBody.ProductName = productName;
            productUninstalledBody.ProductVersion = productVersion;
            productUninstalledBody.Date = date;
            productUninstalledBody.UserId = guid;

            var body = JsonSerializer.Serialize(productUninstalledBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Date is required."));
        }

        [TestCase("", "1.1", "12/12/2023", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd")]
        // Bug: Product name can be empty, and request is successful
        public void ProductUninstalledRequiredProductNameTests(string productName, string productVersion, string date, string guid)
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductUninstalled);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            ProductUninstalledInvalidDto productUninstalledBody = new ProductUninstalledInvalidDto();
            productUninstalledBody.ProductName = productName;
            productUninstalledBody.ProductVersion = productVersion;
            productUninstalledBody.Date = date;
            productUninstalledBody.UserId = guid;

            var body = JsonSerializer.Serialize(productUninstalledBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.Content, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Invalid ProductName format"));
        }

        [TestCase("Product Name", "", "12/12/2023", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd")]
        public void ProductUninstalledRequiredEmptyProductVersionTests(string productName, string productVersion, string date, string guid)
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductUninstalled);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            ProductUninstalledInvalidDto productUninstalledBody = new ProductUninstalledInvalidDto();
            productUninstalledBody.ProductName = productName;
            productUninstalledBody.ProductVersion = productVersion;
            productUninstalledBody.Date = date;
            productUninstalledBody.UserId = guid;

            var body = JsonSerializer.Serialize(productUninstalledBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Incorrect ProductVersion format."));

        }

        [Test]
        // Bug: No date validation for future date
        public void ProductUninstalledFutureDateTests()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductUninstalled);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            ProductUninstalledInvalidDto productUninstalledBody = new ProductUninstalledInvalidDto();
            productUninstalledBody.ProductName = "Test Product Name";
            productUninstalledBody.ProductVersion = "1.0";
            productUninstalledBody.Date = "12/12/3033";
            productUninstalledBody.UserId = Guid.NewGuid().ToString();

            var body = JsonSerializer.Serialize(productUninstalledBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Invalid date"));
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            client.Dispose();
        }
    }
}

