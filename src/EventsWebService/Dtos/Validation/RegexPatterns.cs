namespace EventsWebService.Dtos.Validation
{
    public static class RegexPatterns
    {
        public const string EmailRegex = @"^[\w-\+\']+(\.[\w-\+\']+)*@[A-Za-z0-9]([A-Za-z0-9]|[-.][A-Za-z0-9])*\.((?!ru$|by$)[A-Za-z]{2,})$";

        public const string NoSpecialCharsRegex = @"^[^!@#\$%\^&\*_\(\)\+=\[\]\\;,./\{\}\|:<>\?\x22]+$";

        public const string CompanyNameRegex = @"^(?=.*\S)[^<>]+$";

        public const string PhoneRegex = @"^[a-zA-Z0-9#()\-\+\.\/\*\,\s]*$";

        public const string ProductVersionRegex = @"^(\d+\.)?(\d+\.)?(\*|\d+)$";

    }
}