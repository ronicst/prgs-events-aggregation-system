using System.Net;
using EventsTests.Model;
using EventsTests.Configuration;
using RestSharp;

namespace EventsTests.Common
{
    internal class WepApiRestRequests
    {
        EventsTestRestConnection client;

        public WepApiRestRequests()
        {
            this.client = new EventsTestRestConnection();
        }

        public RestResponse UserLoginStep()
        {
            Console.WriteLine("User Login...");
            // Add query parameter to the URL
            EventType eventName = (EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogin);
            var request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);
            request.AddQueryParameter("type", eventName);

            // Build File Download body
            UserLoginDto userLoginBody = new UserLoginDto();
            userLoginBody.Date = DateTime.Now;
            userLoginBody.Email = "test@test.com";
            userLoginBody.UserId = Guid.NewGuid();
            userLoginBody.FirstName = "Tom";
            userLoginBody.LastName = "Jerry";

            var body = System.Text.Json.JsonSerializer.Serialize(userLoginBody);
            Console.WriteLine(body.ToString());
            request.AddBody(body);

            var response = client.ExecuteRequest(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Console.WriteLine(response.StatusDescription);

            Console.WriteLine("User login ...");
            Console.WriteLine(response.Content.ToString());

            return response;
        }

        public RestResponse UserRegisteredStep()
        {
            Console.WriteLine("User Registered...");
            // Build File Download body
            UserRegisteredDto userRegisteredBody = new UserRegisteredDto();
            userRegisteredBody.Email = "test@test.com";
            userRegisteredBody.FirstName = "Tom";
            userRegisteredBody.LastName = "Jerry";
            userRegisteredBody.Company = "Test Inc";
            userRegisteredBody.Phone = "4234242345676";

            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeUserRegistered);
            var request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);
            request.AddQueryParameter("type", eventValue);

            var body = System.Text.Json.JsonSerializer.Serialize(userRegisteredBody);

            request.AddBody(body);
            var response = client.ExecuteRequest(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Console.WriteLine("User reg ...");
            Console.WriteLine(response.Content.ToString());

            return response;
        }

        public RestResponse FileDownloadStep()
        {
            // Build File Download body
            FileDownloadDto fileDownloadBody = new FileDownloadDto();
            fileDownloadBody.Id = Guid.NewGuid();
            fileDownloadBody.Date = DateTime.Now;
            fileDownloadBody.FileName = "myfile.exe";
            Random rand = new Random();
            fileDownloadBody.FileLenght = rand.Next();

            // Add query parameter to the URL           
            int eventValue = (int)(EventType)Enum.Parse(typeof(EventType), Constants.EventTypeFileDownload);
            var request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);
            request.AddQueryParameter("type", eventValue);

            var body = System.Text.Json.JsonSerializer.Serialize(fileDownloadBody);

            request.AddBody(body);
            var response = client.ExecuteRequest(request);

            return response;
        }

        public RestResponse UserLogoutStep()
        {
            int eventValue = (int)Enum.Parse(typeof(EventType), Constants.EventTypeUserLogout);
            var request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);
            
            // Add query parameter to the URL
            request.AddQueryParameter("type", eventValue);

            UserLogoutDto userLogoutBody = new UserLogoutDto();
            userLogoutBody.Date = DateTime.Now;
            userLogoutBody.Email = "tests@tests.com";
            var body = System.Text.Json.JsonSerializer.Serialize(userLogoutBody);

            Console.WriteLine(body.ToString());

            request.AddBody(body);

            var response = client.ExecuteRequest(request);

            return response;
        }
    }
}
