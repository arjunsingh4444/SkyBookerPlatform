using System.ComponentModel.DataAnnotations;

namespace SkyBooker.FlightService.Dtos;

// Data sent when updating a flight - ID is required, rest are optional
public class UpdateFlightDto
{
    [Required(ErrorMessage = "Flight ID is required")]
    public int Id { get; set; }

    public string? FlightNumber { get; set; }
    public string? AirlineName { get; set; }
    public string? Source { get; set; }
    public string? Destination { get; set; }
    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }

    [Range(1, 100000)]
    public decimal? Price { get; set; }

    [Range(1, 500)]
    public int? TotalSeats { get; set; }

    [Range(0, 500)]
    public int? AvailableSeats { get; set; }

    public string? Status { get; set; }  // "Scheduled", "Delayed", "Cancelled", "Completed"
}
