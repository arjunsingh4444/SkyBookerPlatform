using Microsoft.EntityFrameworkCore;
using SkyBooker.AuthService.Entities;

namespace SkyBooker.AuthService.Data;

// This is the "bridge" between our C# code and SQL Server database.
// It tells Entity Framework: "Hey, I have a Users table"

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    // This represents the "Users" table in the database
    public DbSet<User> Users { get; set; } = null!;

    // Configure table rules (like unique email, max lengths, etc.)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            // Email must be unique - no two users can have the same email
            entity.HasIndex(u => u.Email).IsUnique();

            // Column rules
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Phone).HasMaxLength(20);

            // Store Role as string ("User" or "Admin") instead of number
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        });
    }

    // Automatically update the "UpdatedAt" field whenever we save changes
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
