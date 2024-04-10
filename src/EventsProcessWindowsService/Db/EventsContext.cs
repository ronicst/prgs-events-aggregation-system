using EventsProcessWindowsService.Db.DataObjects;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EventsProcessWindowsService.Db
{
    public class EventsContext : DbContext
    {
        public EventsContext()
        {
            Database.SetInitializer<EventsContext>(null);
        }

        public DbSet<FileDownloadEvent> FileDownloads { get; set; }
        public DbSet<UserLoginEvent> UserLogins { get; set; }
        public DbSet<UserLogOutEvent> UserLogouts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductActionTraking> ProductActions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}