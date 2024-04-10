using EventsTests.Db.DataObjects;
using EventsTests.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EventsTests.Db
{
    public class TestEventsContext : DbContext
    {
        public TestEventsContext() : base("name=TestEventsContext")
        {
            Database.SetInitializer<TestEventsContext>(null);
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