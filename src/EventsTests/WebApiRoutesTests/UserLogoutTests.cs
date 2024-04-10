using System.Text.Json;
using EventsTests.Model;
using RestSharp;
using EventsTests.Common;
using EventsTests.Configuration;

namespace EventsTests.WebApiRoutesTests
{
    internal class UserLogoutTests
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
        public void UserLogoutTestsPositive()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogout);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            UserLogoutDto userLogoutBody = new UserLogoutDto();
            userLogoutBody.Date = DateTime.Now;
            userLogoutBody.Email = "tests@tests.com";
            var body = JsonSerializer.Serialize(userLogoutBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [Test]
        // Bug: No validation for date format, request is successful
        public void UserLogoutTestsInvalidDate()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogout);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            UserLogoutInvalidDto userLogoutBody = new UserLogoutInvalidDto();
            userLogoutBody.Date = "23 Jan 2023";
            userLogoutBody.Email = "tests@tests.com";
            var body = JsonSerializer.Serialize(userLogoutBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [Test]
        // Bug: No validation for Email format, request is successful
        public void UserLogoutTestsInvalidEmail()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogout);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            UserLogoutInvalidDto userLogoutBody = new UserLogoutInvalidDto();
            userLogoutBody.Date = DateTime.Now.ToString();
            userLogoutBody.Email = "Invalid_Email";
            var body = JsonSerializer.Serialize(userLogoutBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [Test]
        // Bug: No validation for Email format, request is successful
        public void UserLogoutTestsEmptyEmail()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogout);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventName);

            UserLogoutInvalidDto userLogoutBody = new UserLogoutInvalidDto();
            userLogoutBody.Date = DateTime.Now.ToString();
            userLogoutBody.Email = "";
            var body = JsonSerializer.Serialize(userLogoutBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            client.Dispose();
        }
    }
}


