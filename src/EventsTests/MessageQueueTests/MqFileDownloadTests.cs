using System.Net;
using EventsTests.Common;
using EventsTests.MessageManager;
using EventsTests.Model;
using RestSharp;
using EventsTests.Configuration;


namespace EventsTests.MessageQueueTests
{
    [TestFixture]
    internal class MqFileDownloadTests
    {
        EventsTestRestConnection client;
        RestRequest request;
        RabbitMQManager mqConnection;

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Starting Rest connection:");
            client = new EventsTestRestConnection();
            request = client.GetApiRequest(Constants.WebApiRouteEvents, Method.Post);

            Console.WriteLine("Creating MQ manager");
            this.mqConnection = new RabbitMQManager();

            /**
             * This suite checks the requests sent by web API are received as messages,
             * clear the queue as setup step
             * in order not to confuse the subsequent tests.
             */
            RabbitMQManager.PurgeQueue();

            Console.WriteLine("Setup done");
        }

        [Test]
        public void FileDownloadPositiveTest()
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
            this.request.AddQueryParameter("type", eventValue);

            var body = System.Text.Json.JsonSerializer.Serialize(fileDownloadBody);

            this.request.AddBody(body);
            var response = client.ExecuteRequest(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Console.WriteLine("Build conssumer to get message from the queue");

            var receivedEvent = this.mqConnection.ConsumeSingleEvent();

            Assert.That(receivedEvent, Is.Not.Null);
            Assert.That(receivedEvent.Type, Is.EqualTo(MessageType.FileDownload));

            var fd = Newtonsoft.Json.JsonConvert.DeserializeObject<FileDownloadDto>(receivedEvent.Data.ToString());
            Assert.That(fd.Date, Is.EqualTo(fileDownloadBody.Date));
            Assert.That(fd.Id, Is.EqualTo(fileDownloadBody.Id));
            Assert.That(fd.FileName, Is.EqualTo(fileDownloadBody.FileName));
            Assert.That(fd.FileLenght, Is.EqualTo(fileDownloadBody.FileLenght));

            Console.WriteLine("Message received from the queue");
        }

        [TearDown]
        public void Teardown()
        {
            this.client.Dispose();
            mqConnection.Dispose();
        }
    }
}
