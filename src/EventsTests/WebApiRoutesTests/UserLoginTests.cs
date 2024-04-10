using System.Net;
using System.Text.Json;
using EventsTests.Model;
using RestSharp;
using EventsTests.Common;
using EventsTests.Configuration;

namespace EventsTests.WebApiRoutesTests
{
    internal class UserLoginTests
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
        public void UserLoginTestsPositive()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogin);
            int eventValue = (int)eventName;

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserLoginDto userLoginBody = new UserLoginDto();
            userLoginBody.Date = DateTime.Now;
            userLoginBody.Email = "tests@tests.com";
            userLoginBody.UserId = Guid.NewGuid();
            userLoginBody.FirstName = "Tom";
            userLoginBody.LastName = "Smith";

            var body = JsonSerializer.Serialize(userLoginBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Assert.That(response.IsSuccessStatusCode, Is.True);

        }

        [Test]
        // Bug: Expected the apropiate error message to be returned
        public void UserLoginTestsInvalidDate()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogin);
            int eventValue = (int)eventName;

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserLoginInvalidDto userLoginBody = new UserLoginInvalidDto();
            userLoginBody.Date = "Invalid date";
            userLoginBody.Email = "tests@tests.com";
            userLoginBody.UserId = Guid.NewGuid().ToString();
            userLoginBody.FirstName = "Tom";
            userLoginBody.LastName = "Smith";

            var body = JsonSerializer.Serialize(userLoginBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Invalid date format"));

        }

        [Test]
        public void UserLoginTestsInvalidEmail()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogin);
            int eventValue = (int)eventName;

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserLoginDto userLoginBody = new UserLoginDto();
            userLoginBody.Date = DateTime.Now;
            userLoginBody.Email = "Invalid_Email";
            userLoginBody.UserId = Guid.NewGuid();
            userLoginBody.FirstName = "Tom";
            userLoginBody.LastName = "Smith";

            var body = JsonSerializer.Serialize(userLoginBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Incorrect email format"));
        }

        [Test]
        // Bug: Expected the apropiate error message to be returned.
        // The error to be more concrete and the message to be more user friendly - none stack trace. 
        public void UserLoginTestsInvalidGuid()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogin);
            int eventValue = (int)eventName;

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserLoginInvalidDto userLoginBody = new UserLoginInvalidDto();
            userLoginBody.Date = DateTime.Now.ToString();
            userLoginBody.Email = "tests@tests.com";
            userLoginBody.UserId = "invalid-GUID";
            userLoginBody.FirstName = "Tom";
            userLoginBody.LastName = "Smith";

            var body = JsonSerializer.Serialize(userLoginBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Incorrect GUID format"));
        }

        [Test]
        // Bug: No restrictions for "FirstName" field size
        public void UserLoginTestsLongFirstName()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogin);
            int eventValue = (int)eventName;

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserLoginDto userLoginBody = new UserLoginDto();
            userLoginBody.Date = DateTime.Now;
            userLoginBody.Email = "tests@tests.com";
            userLoginBody.UserId = Guid.NewGuid();
            GenerateString generateString = new GenerateString();
            userLoginBody.FirstName = generateString.createString(10000, 'b');
            userLoginBody.LastName = "Smith";

            var body = JsonSerializer.Serialize(userLoginBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.False);
        }

        [Test]
        // Bug: No restrictions for "LastName" field size and the request is successful
        public void UserLoginTestsLongLastName()
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogin);
            int eventValue = (int)eventName;

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserLoginDto userLoginBody = new UserLoginDto();
            userLoginBody.Date = DateTime.Now;
            userLoginBody.Email = "tests@tests.com";
            userLoginBody.UserId = Guid.NewGuid();
            userLoginBody.FirstName = "Tom";
            GenerateString generateString = new GenerateString();
            userLoginBody.LastName = generateString.createString(10000, 'a');

            var body = JsonSerializer.Serialize(userLoginBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [TestCase("12/12/2024", "", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "Johny", "Bravo")]
        [TestCase("12/12/2024", "test@test.com", "", "Johny", "Bravo")]
        [TestCase("12/12/2024", "test@test.com", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "", "Bravo")]
        [TestCase("12/12/2024", "test@test.com", "2fd7fd4d-8ee6-ee11-a8f8-f48e38cc0ecd", "Johny", "")]
        // Bug: Email, GUID, First and Last name can be empty
        public void UserLoginTestsRequiredDataTest(string date, string email, string guid, string firstName, string lastName)
        {
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogin);
            int eventValue = (int)eventName;

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserLoginInvalidDto userLoginBody = new UserLoginInvalidDto();
            userLoginBody.Date = date;
            userLoginBody.Email = "tests@tests.com";
            userLoginBody.UserId = Guid.NewGuid().ToString();
            userLoginBody.FirstName = "Tom";
            userLoginBody.LastName = "Smith";

            var body = JsonSerializer.Serialize(userLoginBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            client.Dispose();
        }
    }
}

