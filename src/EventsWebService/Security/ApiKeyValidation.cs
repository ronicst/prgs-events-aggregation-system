namespace EventsWebService.Security
{
    public class ApiKeyValidation : IApiKeyValidation
    {
        private readonly IConfiguration configuration;

        public ApiKeyValidation(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool IsValidApiKey(string userApiKey)
        {
            if (string.IsNullOrWhiteSpace(userApiKey))
            {
                return false;
            }
                
            string? apiKey = configuration.GetValue<string>(Constants.ApiKeyName);
            
            if (apiKey == null || apiKey != userApiKey)
            {
                return false;
            }

            return true;
        }
    }
}