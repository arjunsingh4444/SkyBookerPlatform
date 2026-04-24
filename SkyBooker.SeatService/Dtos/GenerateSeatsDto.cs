using System.ComponentModel.DataAnnotations;

namespace SkyBooker.SeatService.Dtos;

// Used when generating multiple seats at once for a flight
public class GenerateSeatsDto
{
    [Required]
    public int FlightId { get; set; }

    [Required]
    [Range(1, 500)]
    public int TotalSeats { get; set; }         // How many seats to generate

    [Required]
    [Range(1, 100000)]
    public decimal EconomyPrice { get; set; }   // Price for economy class

    public int SeatsPerRow { get; set; } = 6;   // Seats per row (A-F default)
}
