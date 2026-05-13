using Microsoft.EntityFrameworkCore;
using SkyBooker.NotificationService.Entities;

namespace SkyBooker.NotificationService.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<NotificationLog> NotificationLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.Property(n => n.Recipient).IsRequired().HasMaxLength(150);
            entity.Property(n => n.Subject).HasMaxLength(255);
            entity.Property(n => n.Type).HasMaxLength(50);
            entity.Property(n => n.Status).HasMaxLength(50);
        });
    }
}
