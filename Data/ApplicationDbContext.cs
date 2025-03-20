using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarRentalWeb.Models;

namespace CarRentalWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure decimal precision
            builder.Entity<Car>()
                .Property(c => c.DailyRate)
                .HasPrecision(10, 2);

            builder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(10, 2);
        }
    }
}
