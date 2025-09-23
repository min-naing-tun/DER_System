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

        public DbSet<MaterialTypes> MaterialTypes { get; set; }

        public DbSet<Materials> Materials { get; set; }

        public DbSet<Routes> Routes { get; set; }

        public DbSet<CustomerRouteListings> CustomerRouteListings { get; set; }

        public DbSet<CustomerMaterialListings> CustomerMaterialListings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Table mapping
            modelBuilder.Entity<Users>().ToTable(constants.User);
            modelBuilder.Entity<Customers>().ToTable(constants.Customer);
            modelBuilder.Entity<MaterialTypes>().ToTable(constants.MaterialType);
            modelBuilder.Entity<Materials>().ToTable(constants.Material);
            modelBuilder.Entity<Routes>().ToTable(constants.Route);
            modelBuilder.Entity<CustomerRouteListings>().ToTable(constants.CustomerRouteListing);
            modelBuilder.Entity<CustomerMaterialListings>().ToTable(constants.CustomerMaterialListing);

            //Table exclude on migration
            modelBuilder.Entity<Users>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Customers>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<MaterialTypes>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Materials>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Routes>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<CustomerRouteListings>().ToTable(t => t.ExcludeFromMigrations());
            modelBuilder.Entity<CustomerMaterialListings>().ToTable(t => t.ExcludeFromMigrations());
        }
    }
}
