using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EventsTests.Common;
using EventsTests.MessageManager;
using EventsTests.Model;
using Newtonsoft.Json;
using NUnit.Framework.Constraints;
using RestSharp;
using EventsTests.Configuration;

namespace EventsTests.WebApiRoutesTests
{
    internal class FileDownloadTests
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
        public void EventsPositive()
        {
            //var request = client.GetApiRequest("Events", Method.Post);

            FileDownloadDto fileDownloadBody = new FileDownloadDto();
            fileDownloadBody.Id = Guid.NewGuid();
            fileDownloadBody.Date = DateTime.Now;

            fileDownloadBody.FileName = "myfile.exe";
            fileDownloadBody.FileLenght = 252588;

            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeFileDownload);
            request.AddQueryParameter("type", eventValue);

            Console.WriteLine(fileDownloadBody.Id.ToString());

            // Add body to the request
            var body = System.Text.Json.JsonSerializer.Serialize(fileDownloadBody);
            request.AddBody(body);
            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public void EventsInvalidFileLenght()
        {
            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeFileDownload);
            Console.WriteLine(eventValue);
            this.request.AddQueryParameter("type", eventValue);

            FileDownloadInvalidDto fileDownloadBody = new FileDownloadInvalidDto();
            fileDownloadBody.Id = Guid.NewGuid().ToString();
            fileDownloadBody.Date = DateTime.Now.ToString();
            fileDownloadBody.FileName = "myfile.exe";
            fileDownloadBody.FileLenght = "0";

            Console.WriteLine(fileDownloadBody.Id.ToString());

            var body = System.Text.Json.JsonSerializer.Serialize(fileDownloadBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("FileLenght must be positive integer."));

        }

        [Test]
        public void EventsFileNameMaxLenght()
        {
            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeFileDownload);
            Console.WriteLine(eventValue);
            request.AddQueryParameter("type", eventValue);

            FileDownloadDto fileDownloadBody = new FileDownloadDto();
            fileDownloadBody.Id = Guid.NewGuid();
            fileDownloadBody.Date = DateTime.Now;
            GenerateString generateString = new GenerateString();
            fileDownloadBody.FileName = generateString.createString(2001, 'a');
            fileDownloadBody.FileLenght = 32342;

            var body = System.Text.Json.JsonSerializer.Serialize(fileDownloadBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.EqualTo("[\"FileName over max lenght.\"]"));

        }

        [Test]
        public void EventsFileLenghtMaxSize()
        {
            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeFileDownload);
            Console.WriteLine(eventValue);
            request.AddQueryParameter("type", eventValue);

            FileDownloadInvalidDto fileDownloadBody = new FileDownloadInvalidDto();
            fileDownloadBody.Id = Guid.NewGuid().ToString();
            fileDownloadBody.Date = DateTime.Now.ToString();
            GenerateString generateString = new GenerateString();
            fileDownloadBody.FileName = generateString.createString(20, 'a');
            fileDownloadBody.FileLenght = generateString.createString(1000, '9');

            var body = System.Text.Json.JsonSerializer.Serialize(fileDownloadBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Unexpected error"));

        }

        [Test]
        // Bug: The API accepts the date in the future, there isn't a validation for this.
        public void EventsDateInFuture()
        {
            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeFileDownload);
            Console.WriteLine(eventValue);
            request.AddQueryParameter("type", eventValue);

            FileDownloadInvalidDto fileDownloadBody = new FileDownloadInvalidDto();
            fileDownloadBody.Id = Guid.NewGuid().ToString();
            fileDownloadBody.Date = "3/27/3024";
            fileDownloadBody.FileName = "myfile.exe";
            fileDownloadBody.FileLenght = "5656";

            Console.WriteLine(fileDownloadBody.Id.ToString());

            var body = System.Text.Json.JsonSerializer.Serialize(fileDownloadBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        }

        [TestCase(" ", "2/22/2024", "myfile.exe", "3343")]
        [TestCase("2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "", "myfile.exe", "3343")]
        [TestCase("2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "3/27/3024", "", "3343")]
        [TestCase("2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "3/27/3024", "myfile.exe", "-1")]
        [TestCase("2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "3/27/3024", "myfile.exe", "")]
        // Bug: File name is not mandatory field
        public void EventsRequiredData(string guid, string date, string fileName, string fileLenght)
        {
            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeFileDownload);
            Console.WriteLine(eventValue);
            request.AddQueryParameter("type", eventValue);

            FileDownloadInvalidDto fileDownloadBody = new FileDownloadInvalidDto();
            fileDownloadBody.Id = guid;
            fileDownloadBody.Date = date;
            fileDownloadBody.FileName = fileName;
            fileDownloadBody.FileLenght = fileLenght;

            Console.WriteLine(fileDownloadBody.Id.ToString());

            var body = System.Text.Json.JsonSerializer.Serialize(fileDownloadBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            client.Dispose();
        }
    }
}
