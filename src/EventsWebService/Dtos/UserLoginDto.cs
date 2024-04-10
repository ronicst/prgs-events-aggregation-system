using EventsWebService.Dtos.Validation;
using System.Text.RegularExpressions;

namespace EventsWebService.Dtos
{
    public class UserLoginDto : IEventDto
    {
        public DateTime Date { get; set; }

        public string Email { get; set; }

        public Guid? UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string[] Validate()
        {
            List<string> errors = new List<string>();

            if (this.Date == DateTime.MinValue)
            {
                errors.Add("Date is required.");
            }

            if (this.Email == null)
            {
                errors.Add("Email is required.");
            }

            if (!(!string.IsNullOrEmpty(this.Email) && Regex.IsMatch(this.Email, RegexPatterns.EmailRegex)))
            {
                errors.Add("Incorrect email format.");
            }

            if (this.Email != null && this.Email.Length > 150)
            {
                errors.Add("Email must be less than 150 charecters long.");
            }

            if (this.UserId == null || this.UserId == Guid.Empty)
            {
                errors.Add("UserId is required.");
            }

            return errors.ToArray();
        }
    }
}