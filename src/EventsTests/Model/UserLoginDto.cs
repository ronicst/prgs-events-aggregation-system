namespace EventsTests.Model
{
    public class UserLoginDto
    {
        public DateTime Date { get; set; }

        public string Email { get; set; }

        public Guid? UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}