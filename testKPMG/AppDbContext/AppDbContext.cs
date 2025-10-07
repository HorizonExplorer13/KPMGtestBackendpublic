using Microsoft.EntityFrameworkCore;
using testKPMG.Entities;

namespace testKPMG.AppDbContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected AppDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(10, 2);
        }

        public DbSet<Product> products { get; set; }
    }
}
