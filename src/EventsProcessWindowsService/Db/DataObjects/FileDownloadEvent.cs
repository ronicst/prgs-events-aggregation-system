namespace EventsProcessWindowsService.Db.DataObjects
{
    public class FileDownloadEvent
    {
        public int Id { get; set; }

        public string EventId { get; set; }

        public string Date { get; set; }

        public string FileName { get; set; }

        public int FileLenght { get; set; }
    }
}