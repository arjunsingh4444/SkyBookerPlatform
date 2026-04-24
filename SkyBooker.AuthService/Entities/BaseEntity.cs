namespace SkyBooker.AuthService.Entities;

// Base class that all entities (User, etc.) inherit from.
// Gives every entity an ID and timestamp fields automatically.

public abstract class BaseEntity
{
    public int Id { get; set; }                                  // Auto-increment numeric ID (1, 2, 3...)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;   // When the record was created
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;   // When the record was last updated
}
