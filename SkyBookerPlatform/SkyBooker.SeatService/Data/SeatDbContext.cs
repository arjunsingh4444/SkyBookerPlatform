using Microsoft.EntityFrameworkCore;
using SkyBooker.SeatService.Entities;

namespace SkyBooker.SeatService.Data;

public class SeatDbContext : DbContext
{
    public SeatDbContext(DbContextOptions<SeatDbContext> options) : base(options) { }

    public DbSet<Seat> Seats { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Seat>(entity =>
        {
            // Same seat number can't exist twice on the same flight
            entity.HasIndex(s => new { s.FlightId, s.SeatNumber }).IsUnique();

            entity.Property(s => s.SeatNumber).IsRequired().HasMaxLength(10);
            entity.Property(s => s.SeatClass).IsRequired().HasMaxLength(20);
            entity.Property(s => s.Price).HasColumnType("decimal(10,2)");
            entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
