using RestSharp;
using EventsTests.Configuration;

namespace EventsTests.Common
{
    internal class EventsTestRestConnection : IDisposable
    {
        // XXX-MIB: Get the url from appSetings
        private RestClient client = new RestClient(Constants.ApiUri);

        public RestRequest GetApiRequest(string uri, Method restMethod)
        {
            RestRequest request = new RestRequest(uri, restMethod);
            request.AddHeader("accept", Constants.Accept);
            request.AddHeader("Content-Type", Constants.ContentType);
            request.AddHeader(Constants.ApiKeyHeaderName, Constants.ApiKeyValue);

            return request;
        }

        public RestResponse ExecuteRequest(RestRequest request)
        {
            return client.Execute(request);
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}