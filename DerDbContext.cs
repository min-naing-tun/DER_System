using DER_System.Model;
using DER_System.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DER_System
{
    public class DerDbContext : DbContext
    {
        Constants constants = new Constants();

        public DerDbContext(DbContextOptions<DerDbContext> options) : base(options)
        {

        }

        public DbSet<API_Logs> API_Logs { get; set; }

        public DbSet<Users> Users { get; set; }

        public DbSet<Customers> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Table mapping
            modelBuilder.Entity<Users>().ToTable(constants.User);
            modelBuilder.Entity<Customers>().ToTable(constants.Customer);

            //Table exclude on migration
            modelBuilder.Entity<Users>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Customers>().ToTable(t => t.ExcludeFromMigrations());
        }
    }
}
