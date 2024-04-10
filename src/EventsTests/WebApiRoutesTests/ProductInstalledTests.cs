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
using System.Reflection.Metadata;
using EventsTests.Configuration;

namespace EventsTests.WebApiRoutesTests
{
    internal class ProductInstalledTests
    {
        EventsTestRestConnection client;

        [SetUp]
        public void Setup()
        {
            client = new EventsTestRestConnection();
            Console.WriteLine("On setup");
        }

        [Test]
        public void ProductInstalledRestTestsPositive()
        {
            var request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);

            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductInstalled);
            request.AddQueryParameter("type", eventName);

            ProductInstalledDto productInstalledBody = new ProductInstalledDto();
            productInstalledBody.ProductName = "Test Product Name";
            productInstalledBody.ProductVersion = "1.0";    
            productInstalledBody.Date = DateTime.Now;
            productInstalledBody.UserId = Guid.NewGuid();

            var body = JsonSerializer.Serialize(productInstalledBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.That(response.Content, Contains.Substring("Event processed successfully"));

        }

        [Test]
        public void ProductInstalledRestInvalidDateTest()
        {
            var request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);

            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductInstalled);
            request.AddQueryParameter("type", eventName);

            ProductInstalledInvalidDto productInstalledBody = new ProductInstalledInvalidDto();
            productInstalledBody.ProductName = "Test Product Name";
            productInstalledBody.ProductVersion = "1.0";
            productInstalledBody.Date = "Invalid date";
            productInstalledBody.UserId = Guid.NewGuid().ToString();

            var body = JsonSerializer.Serialize(productInstalledBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Invalid date"));

        }

        [Test]
        public void ProductInstalledRestInvalidGuidTest()
        {
            var request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);

            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductInstalled);
            request.AddQueryParameter("type", eventName);

            ProductInstalledInvalidDto productInstalledBody = new ProductInstalledInvalidDto();
            productInstalledBody.ProductName = "Test Product Name";
            productInstalledBody.ProductVersion = "1.0";
            productInstalledBody.Date = DateTime.Now.ToString();
            productInstalledBody.UserId = "Fake-GUID";

            var body = JsonSerializer.Serialize(productInstalledBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Error converting value"));

        }

        [Test]
        // Bug:  there isn’t limitation of the field size
        public void ProductInstalledRestLongProductNameTest()
        {
            var request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);

            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductInstalled);
            request.AddQueryParameter("type", eventName);

            ProductInstalledInvalidDto productInstalledBody = new ProductInstalledInvalidDto();
            GenerateString generateString = new GenerateString();
            productInstalledBody.ProductName = generateString.createString(10000, 'a');
            productInstalledBody.ProductVersion = "1.0";
            productInstalledBody.Date = DateTime.Now.ToString();
            productInstalledBody.UserId = Guid.NewGuid().ToString();

            var body = JsonSerializer.Serialize(productInstalledBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public void ProductInstalledRestLongProductVersionTest()
        {
            var request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);

            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeProductInstalled);
            request.AddQueryParameter("type", eventName);

            ProductInstalledInvalidDto productInstalledBody = new ProductInstalledInvalidDto();
            
            productInstalledBody.ProductName = "Test Product Name";
            GenerateString generateString = new GenerateString();
            productInstalledBody.ProductVersion = generateString.createString(1000, 'a');
            productInstalledBody.Date = DateTime.Now.ToString();
            productInstalledBody.UserId = Guid.NewGuid().ToString();

            var body = JsonSerializer.Serialize(productInstalledBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Incorrect email format."));

        }

        [OneTimeTearDown]
        public void Teardown()
        {
            client.Dispose();
        }
    }
}

