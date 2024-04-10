namespace EventsWebService.Dtos
{
    public class UserLogoutDto : IEventDto
    {
        public DateTime Date { get; set; }

        public string Email { get; set; }

        public string[] Validate()
        {
            return [];
        }
    }
}