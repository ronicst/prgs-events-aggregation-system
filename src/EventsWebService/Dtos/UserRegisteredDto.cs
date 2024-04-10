using EventsWebService.Dtos.Validation;
using System.Text.RegularExpressions;

namespace EventsWebService.Dtos
{
    public class UserRegisteredDto : IEventDto
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Company { get; set; }

        public string Phone { get; set; }

        public string[] Validate()
        {
            List<string> errors = new List<string>();

            if (this.Email == null)
            {
                errors.Add("Email is required.");
            }

            if (this.FirstName == null)
            {
                errors.Add("FirstName is required.");
            }

            if (this.LastName == null)
            {
                errors.Add("LastName is required.");
            }

            if (this.Company == null)
            {
                errors.Add("Company is required.");
            }

            if (this.Phone == null)
            {
                errors.Add("Phone is required.");
            }

            if (!(!string.IsNullOrEmpty(this.Email) && Regex.IsMatch(this.Email, RegexPatterns.EmailRegex)))
            {
                errors.Add("Incorrect email format");
            }

            if (this.Email != null && this.Email.Length > 150)
            {
                errors.Add("Email must be less than 150 charecters long.");
            }

            if (this.FirstName != null && this.FirstName.Length > 50)
            {
                errors.Add("FirstName must be less than 50 charecters long.");
            }

            if (this.LastName != null && this.LastName.Length > 50)
            {
                errors.Add("FirstName must be less than 50 charecters long.");
            }

            if (!(!string.IsNullOrEmpty(this.Phone) && Regex.IsMatch(this.Phone, RegexPatterns.PhoneRegex)))
            {
                errors.Add("Incorrect Phone format.");
            }

            if (!(!string.IsNullOrEmpty(this.Company) && Regex.IsMatch(this.Company, RegexPatterns.CompanyNameRegex)))
            {
                errors.Add("Incorrect Company format.");
            }

            return errors.ToArray();
        }
    }
}