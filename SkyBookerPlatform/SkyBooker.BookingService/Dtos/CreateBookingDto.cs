using System.ComponentModel.DataAnnotations;

namespace SkyBooker.BookingService.Dtos;

public class CreateBookingDto
{
    [Required]
    public int FlightId { get; set; }

    [Required]
    public string SeatNumber { get; set; } = string.Empty;

    [Required]
    [Range(1, 100000)]
    public decimal TotalAmount { get; set; }

    [Required]
    public string PassengerName { get; set; } = string.Empty;

    [Required]
    public string PassengerEmail { get; set; } = string.Empty;

    public string PassengerPhone { get; set; } = string.Empty;

    public string? Status { get; set; } // Optional: Confirmed or Pending
}
