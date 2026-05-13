namespace SkyBooker.SeatService.Entities;

// Seat entity → represents a row in the "Seats" table
// Each seat belongs to a flight (by FlightId)

public class Seat : BaseEntity
{
    public int FlightId { get; set; }                              // Which flight this seat belongs to
    public string SeatNumber { get; set; } = string.Empty;         // e.g. "1A", "12B", "25F"
    public string SeatClass { get; set; } = "Economy";             // Economy, Business, First
    public decimal Price { get; set; }                              // Price for this specific seat
    public SeatStatus Status { get; set; } = SeatStatus.Available;
    public int? LockedByUserId { get; set; }                        // Who locked it (null if available)
    public DateTime? LockExpiresAt { get; set; }                    // When the lock expires
    public int? BookedByUserId { get; set; }                        // Who booked it (null if not booked)
}
