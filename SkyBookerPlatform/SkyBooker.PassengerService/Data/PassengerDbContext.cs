using Microsoft.EntityFrameworkCore;
using SkyBooker.PassengerService.Entities;

namespace SkyBooker.PassengerService.Data;

public class PassengerDbContext : DbContext
{
    public PassengerDbContext(DbContextOptions<PassengerDbContext> options) : base(options) { }

    public DbSet<Passenger> Passengers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Passenger>(entity =>
        {
            entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.PassportNumber).HasMaxLength(20);
            entity.Property(p => p.Nationality).HasMaxLength(50);
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
