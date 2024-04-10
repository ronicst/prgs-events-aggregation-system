namespace EventsWebService.Dtos
{
    public enum EventType
    {
        None = 0,
        FileDownload = 1,
        UserLogin = 2,
        UserRegistered = 3,
        UserLogout = 4,
        FileUpload = 5,
        ProductInstalled = 6,
        ProductUninstalled = 7
    }
}