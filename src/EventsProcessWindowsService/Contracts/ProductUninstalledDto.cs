using System;

namespace EventsProcessWindowsService.Contracts
{
    public class ProductUninstalledDto
    {
        public string ProductName { get; set; }

        public string ProductVersion { get; set; }

        public Guid UserId { get; set; }

        public DateTime Date { get; set; }
    }
}