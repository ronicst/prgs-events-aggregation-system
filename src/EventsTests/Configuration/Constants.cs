namespace EventsTests.Configuration;

// TODO: Consider which constants to be migrated to app settings
public static class Constants
{
    // API key authentication
    public const string ApiKeyHeaderName = "X-API-Key";
    public const string ApiKeyName = "ApiKey";
    public const string ApiKeyValue = "eyJSb2xlIjoiQWRtaW4iLCJJc3N1ZXIiOiJJc3N1ZXIiLCJVc2VybmFtZSI6IkphdmFJblVzZSIsImV4cCI6MTcxMTYzMTQ4MywiaWF0IjoxNzExNjMxNDgzfQ";

    // Web API URI
    public const string ApiUri = "http://localhost:5083";

    // Web API routes
    public const string WebApiRouteEvents = "events";
    public const string WebApiRouteEnvironments = "environment";
    public const string WebApiRouteUsers = "users";

    // Events types
    public const string EventTypeFileDownload = "FileDownload";
    public const string EventTypeUserLogin = "UserLogin";
    public const string EventTypeUserRegistered = "UserRegistered";
    public const string EventTypeUserLogout = "UserLogout";
    public const string EventTypeFileUpload = "FileUpload";
    public const string EventTypeProductInstalled = "ProductInstalled";
    public const string EventTypeProductUninstalled = "ProductUninstalled";

    // Request headers
    public const string Accept = "*/*";
    public const string ContentType = "application/json";

    // RabbitMQ connection configuration
    public const string MqHostName = "localhost";
    public const string MqUserName = "guest";
    public const string MqPassword = "guest";
    public const string VirtualHost = "/";
    public const string MqPort = "5672";
    
}