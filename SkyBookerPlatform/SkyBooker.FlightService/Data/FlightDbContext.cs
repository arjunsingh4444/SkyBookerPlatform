using Microsoft.EntityFrameworkCore;
using SkyBooker.FlightService.Entities;

namespace SkyBooker.FlightService.Data;

// Database context for Flight Service - connects to SQL Server "SkyBookerFlightDb"

public class FlightDbContext : DbContext
{
    public FlightDbContext(DbContextOptions<FlightDbContext> options) : base(options) { }

    // This represents the "Flights" table in the database
    public DbSet<Flight> Flights { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Flight>(entity =>
        {
            // Flight number must be unique
            entity.HasIndex(f => f.FlightNumber).IsUnique();

            entity.Property(f => f.FlightNumber).IsRequired().HasMaxLength(20);
            entity.Property(f => f.AirlineName).IsRequired().HasMaxLength(100);
            entity.Property(f => f.Source).IsRequired().HasMaxLength(100);
            entity.Property(f => f.Destination).IsRequired().HasMaxLength(100);
            entity.Property(f => f.Price).HasColumnType("decimal(10,2)");
            entity.Property(f => f.Status).HasConversion<string>().HasMaxLength(20);
        });
    }

    // Auto-update the "UpdatedAt" field whenever we save changes
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
