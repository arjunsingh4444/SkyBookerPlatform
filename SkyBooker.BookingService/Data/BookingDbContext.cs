using Microsoft.EntityFrameworkCore;
using SkyBooker.BookingService.Entities;

namespace SkyBooker.BookingService.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(b => b.PNR).IsUnique();
            entity.Property(b => b.PNR).IsRequired().HasMaxLength(15);
            entity.Property(b => b.SeatNumber).IsRequired().HasMaxLength(10);
            entity.Property(b => b.TotalAmount).HasColumnType("decimal(10,2)");
            entity.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);
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
