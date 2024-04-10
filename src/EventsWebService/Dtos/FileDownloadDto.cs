namespace EventsWebService.Dtos
{
    public class FileDownloadDto : IEventDto
    {
        public Guid? Id { get; set; }

        public DateTime Date { get; set; }

        public string FileName { get; set; }

        public int FileLenght { get; set; }

        public string[] Validate()
        {
            List<string> errors = new List<string>();

            if (this.Id == null)
            {
                errors.Add("Id is required.");
            }

            if (this.Date == DateTime.MinValue)
            {
                errors.Add("Date is required.");
            }

            if (this.FileName == null)
            {
                errors.Add("FileName is required.");
            }

            if (this.FileName != null && this.FileName.Length > 2000)
            {
                errors.Add("FileName over max lenght.");
            }

            if (this.FileLenght <= 0)
            {
                errors.Add("FileLenght must be positive integer.");
            }

            return errors.ToArray();
        }
    }
}