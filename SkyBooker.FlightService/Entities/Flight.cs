namespace SkyBooker.FlightService.Entities;

// Flight entity → represents a row in the "Flights" table

public class Flight : BaseEntity
{
    public string FlightNumber { get; set; } = string.Empty;       // e.g. "SK-101"
    public string AirlineName { get; set; } = string.Empty;        // e.g. "SkyBooker Airlines"
    public string Source { get; set; } = string.Empty;              // Departure city (e.g. "Delhi")
    public string Destination { get; set; } = string.Empty;         // Arrival city (e.g. "Mumbai")
    public DateTime DepartureTime { get; set; }                     // When the flight departs
    public DateTime ArrivalTime { get; set; }                       // When the flight arrives
    public decimal Price { get; set; }                               // Ticket price
    public int TotalSeats { get; set; }                              // Total seats in the flight
    public int AvailableSeats { get; set; }                          // Seats still available
    public FlightStatus Status { get; set; } = FlightStatus.Scheduled;
}
