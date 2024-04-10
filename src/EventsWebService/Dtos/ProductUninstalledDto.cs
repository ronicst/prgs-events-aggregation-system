using EventsWebService.Dtos.Validation;
using System.Text.RegularExpressions;

namespace EventsWebService.Dtos
{
    public class ProductUninstalledDto : IEventDto
    {
        public string ProductName { get; set; }

        public string ProductVersion { get; set; }

        public Guid UserId { get; set; }

        public DateTime? Date {  get; set; }

        public string[] Validate()
        {
            List<string> errors = new List<string>();

            if (this.Date == null)
            {
                errors.Add("Date is required.");
            }

            if (this.ProductName == null)
            {
                errors.Add("ProductName is required.");
            }

            if (this.ProductVersion == null)
            {
                errors.Add("ProductVersion is required.");
            }

            if (!(!string.IsNullOrEmpty(this.ProductVersion) && Regex.IsMatch(this.ProductVersion, RegexPatterns.ProductVersionRegex)))
            {
                errors.Add("Incorrect ProductVersion format.");
            }

            return errors.ToArray();
        }
    }
}