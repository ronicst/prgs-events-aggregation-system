namespace EventsProcessWindowsService.Db.DataObjects
{
    public class ProductActionTraking
    {
        public int Id { get; set; }

        public string ProductName { get; set; }

        public string ProductVersion { get; set; }

        public string ActionType { get; set; }

        public string UserId { get; set; }

        public string Date { get; set; }
    }
}