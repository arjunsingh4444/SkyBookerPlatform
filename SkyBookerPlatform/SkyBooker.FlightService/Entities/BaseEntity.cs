namespace SkyBooker.FlightService.Entities;

// Base class for all entities - gives ID and timestamps automatically

public abstract class BaseEntity
{
    public int Id { get; set; }                                  // Auto-increment numeric ID
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
