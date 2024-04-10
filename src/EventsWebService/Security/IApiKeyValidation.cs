namespace EventsWebService.Security
{
    public interface IApiKeyValidation
    {
        bool IsValidApiKey(string userApiKey);
    }
}