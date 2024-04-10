using System.Net;
using System.Text.Json;
using EventsTests.Model;
using RestSharp;
using EventsTests.Common;
using EventsTests.Configuration;

namespace EventsTests.WebApiRoutesTests
{
    internal class UserRegisteredTests
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
        public void RegisterUserPositive()
        {
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserRegistered);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserRegisteredDto userRegistrationBody = new UserRegisteredDto();
            userRegistrationBody.Email = "tests@tests.com";
            userRegistrationBody.FirstName = "John";
            userRegistrationBody.LastName = "Smith";
            userRegistrationBody.Company = "Test Inc";
            userRegistrationBody.Phone = "1234567890";

            var body = JsonSerializer.Serialize(userRegistrationBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [TestCase("", "John", "Smith", "Test Inc", "4424234242")]
        public void RegisterUserEmptyEmail(string email, string firstName, string lastName, string company, string phone)
        {
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserRegistered);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserRegisteredDto userRegistrationBody = new UserRegisteredDto();
            userRegistrationBody.Email = email;
            userRegistrationBody.FirstName = firstName; 
            userRegistrationBody.LastName = lastName;
            userRegistrationBody.Company = company;
            userRegistrationBody.Phone = phone;

            var body = JsonSerializer.Serialize(userRegistrationBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Incorrect email format"));
        }


        [TestCase("test@test.com", "", "Smith", "Test Inc", "4424234242")]
        public void RegisterUserEmptyFirstName(string email, string firstName, string lastName, string company, string phone)
        {
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserRegistered);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserRegisteredDto userRegistrationBody = new UserRegisteredDto();
            userRegistrationBody.Email = email;
            userRegistrationBody.FirstName = firstName;
            userRegistrationBody.LastName = lastName;
            userRegistrationBody.Company = company;
            userRegistrationBody.Phone = phone;

            var body = JsonSerializer.Serialize(userRegistrationBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [TestCase("test@test.com", "John", "", "Test Inc", "4424234242")]
        public void RegisterUserEmptyLastName(string email, string firstName, string lastName, string company, string phone)
        {
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserRegistered);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserRegisteredDto userRegistrationBody = new UserRegisteredDto();
            userRegistrationBody.Email = email;
            userRegistrationBody.FirstName = firstName;
            userRegistrationBody.LastName = lastName;
            userRegistrationBody.Company = company;
            userRegistrationBody.Phone = phone;

            var body = JsonSerializer.Serialize(userRegistrationBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
        }


        [TestCase("test@test.com", "John", "Smith", "", "4424234242")]
        public void RegisterUserEmptyCompany(string email, string firstName, string lastName, string company, string phone)
        {
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserRegistered);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserRegisteredDto userRegistrationBody = new UserRegisteredDto();
            userRegistrationBody.Email = email;
            userRegistrationBody.FirstName = firstName;
            userRegistrationBody.LastName = lastName;
            userRegistrationBody.Company = company;
            userRegistrationBody.Phone = phone;

            var body = JsonSerializer.Serialize(userRegistrationBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Incorrect Company format"));
        }

        [TestCase("test@test.com", "John", "Smith", "Test Inc", "")]
        public void RegisterUserEmptyPhoneNumber(string email, string firstName, string lastName, string company, string phone)
        {
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserRegistered);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserRegisteredDto userRegistrationBody = new UserRegisteredDto();
            userRegistrationBody.Email = email;
            userRegistrationBody.FirstName = firstName;
            userRegistrationBody.LastName = lastName;
            userRegistrationBody.Company = company;
            userRegistrationBody.Phone = phone;

            var body = JsonSerializer.Serialize(userRegistrationBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Incorrect Phone format"));
        }

        [TestCase("test@test.com", "John", "Smith", "Test Inc", "Phone Number")]
        // Bug: the field Phone accepts plain text, which is not a valid phone number format
        public void RegisterUserPhoneNumberTexts(string email, string firstName, string lastName, string company, string phone)
        {
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserRegistered);

            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserRegisteredDto userRegistrationBody = new UserRegisteredDto();
            userRegistrationBody.Email = email;
            userRegistrationBody.FirstName = firstName;
            userRegistrationBody.LastName = lastName;
            userRegistrationBody.Company = company;
            userRegistrationBody.Phone = phone;

            var body = JsonSerializer.Serialize(userRegistrationBody);

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            Console.WriteLine(response.StatusCode.ToString());
            Console.WriteLine(response.Content);

            Assert.That(response.IsSuccessStatusCode, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Contains.Substring("Incorrect Phone format"));
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            client.Dispose();
        }
    }
}
