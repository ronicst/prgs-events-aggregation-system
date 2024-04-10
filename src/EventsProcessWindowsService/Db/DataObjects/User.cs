namespace EventsProcessWindowsService.Db.DataObjects
{
    public class User
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserCompanyName { get; set; }

        public string DateRegistered { get; set; }
    }
}