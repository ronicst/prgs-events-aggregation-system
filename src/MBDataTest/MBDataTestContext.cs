using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using MBDataTest.Model.DataObjects;

namespace MBDataTest
{
    internal class MBDataTestContext : DbContext
    {
        public MBDataTestContext()
        {
            Database.SetInitializer<MBDataTestContext>(null);
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
