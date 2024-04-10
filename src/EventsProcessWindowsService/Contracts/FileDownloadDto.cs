using System;

namespace EventsProcessWindowsService.Contracts
{
    public class FileDownloadDto
    {
        public Guid Id { get; set; }
        
        public DateTime Date { get; set; }

        public string FileName { get; set; }

        public int FileLenght { get; set; }
    }
}