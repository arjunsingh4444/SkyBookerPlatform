namespace SkyBooker.BookingService.Entities;

public class Booking : BaseEntity
{
    public string PNR { get; set; } = string.Empty; // e.g. "SKY-A83JF"
    public int UserId { get; set; }
    public int FlightId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string PassengerName { get; set; } = string.Empty;
    public string PassengerEmail { get; set; } = string.Empty;
    public string PassengerPhone { get; set; } = string.Empty;
}
