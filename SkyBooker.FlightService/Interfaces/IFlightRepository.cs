using SkyBooker.FlightService.Entities;

namespace SkyBooker.FlightService.Interfaces;

// Contract for database operations on Flights
public interface IFlightRepository
{
    Task<List<Flight>> GetAllAsync();
    Task<Flight?> GetByIdAsync(int id);
    Task<Flight?> GetByFlightNumberAsync(string flightNumber);
    Task<List<Flight>> SearchAsync(string? source, string? destination, DateTime? date);
    Task<Flight> CreateAsync(Flight flight);
    Task<Flight> UpdateAsync(Flight flight);
    Task<bool> DeleteAsync(int id);
}
