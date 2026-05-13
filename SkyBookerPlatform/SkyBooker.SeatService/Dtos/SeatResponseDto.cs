namespace SkyBooker.SeatService.Dtos;

// Seat data sent to the client
public class SeatResponseDto
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string SeatClass { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;       // Available, Locked, Booked
    public int? LockedByUserId { get; set; }
    public int? BookedByUserId { get; set; }
}
