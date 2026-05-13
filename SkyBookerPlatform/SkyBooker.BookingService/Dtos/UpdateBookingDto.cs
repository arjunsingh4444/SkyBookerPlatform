using System.ComponentModel.DataAnnotations;

namespace SkyBooker.BookingService.Dtos;

public class UpdateBookingDto
{
    public string? PassengerName { get; set; }
    public string? SeatNumber { get; set; }
    public string? Status { get; set; }
}
