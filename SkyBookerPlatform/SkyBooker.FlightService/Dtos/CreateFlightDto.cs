using System.ComponentModel.DataAnnotations;

namespace SkyBooker.FlightService.Dtos;

// Data sent when creating a new flight (admin only)
public class CreateFlightDto
{
    [Required(ErrorMessage = "Flight number is required")]
    public string FlightNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Airline name is required")]
    public string AirlineName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Source city is required")]
    public string Source { get; set; } = string.Empty;

    [Required(ErrorMessage = "Destination city is required")]
    public string Destination { get; set; } = string.Empty;

    [Required(ErrorMessage = "Departure time is required")]
    public DateTime DepartureTime { get; set; }

    [Required(ErrorMessage = "Arrival time is required")]
    public DateTime ArrivalTime { get; set; }

    [Required]
    [Range(1, 100000, ErrorMessage = "Price must be between 1 and 100000")]
    public decimal Price { get; set; }

    [Required]
    [Range(1, 500, ErrorMessage = "Total seats must be between 1 and 500")]
    public int TotalSeats { get; set; }
}
