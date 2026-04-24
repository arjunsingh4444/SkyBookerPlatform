using System.ComponentModel.DataAnnotations;

namespace SkyBooker.SeatService.Dtos;

// Used when admin adds seats to a flight
public class CreateSeatDto
{
    [Required]
    public int FlightId { get; set; }

    [Required(ErrorMessage = "Seat number is required")]
    public string SeatNumber { get; set; } = string.Empty;   // e.g. "1A"

    public string SeatClass { get; set; } = "Economy";        // Economy, Business, First

    [Required]
    [Range(1, 100000)]
    public decimal Price { get; set; }
}
