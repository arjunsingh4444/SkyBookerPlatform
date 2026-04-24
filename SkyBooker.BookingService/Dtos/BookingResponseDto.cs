namespace SkyBooker.BookingService.Dtos;

public class BookingResponseDto
{
    public int Id { get; set; }
    public string PNR { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int FlightId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
