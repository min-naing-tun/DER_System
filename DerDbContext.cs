using DER_System.Model;
using Microsoft.EntityFrameworkCore;

namespace DER_System
{
    public class DerDbContext : DbContext
    {
        public DerDbContext(DbContextOptions<DerDbContext> options) : base(options)
        {

        }

        public DbSet<API_Logs> API_Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
