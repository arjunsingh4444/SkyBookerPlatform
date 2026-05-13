using Microsoft.EntityFrameworkCore;
using SkyBooker.FlightService.Data;
using SkyBooker.FlightService.Entities;
using SkyBooker.FlightService.Interfaces;

namespace SkyBooker.FlightService.Repository;

// Database operations for Flights using Entity Framework

public class FlightRepository : IFlightRepository
{
    private readonly FlightDbContext _context;

    public FlightRepository(FlightDbContext context)
    {
        _context = context;
    }

    // Get all flights (only upcoming)
    public async Task<List<Flight>> GetAllAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Flights
            .Where(f => f.DepartureTime > now)
            .OrderBy(f => f.DepartureTime)
            .ToListAsync();
    }

    // Find flight by ID
    public async Task<Flight?> GetByIdAsync(int id)
    {
        return await _context.Flights.FindAsync(id);
    }

    // Find flight by flight number (e.g. "SK-101")
    public async Task<Flight?> GetByFlightNumberAsync(string flightNumber)
    {
        return await _context.Flights
            .FirstOrDefaultAsync(f => f.FlightNumber.ToLower() == flightNumber.ToLower());
    }

    // Search flights by source, destination, and/or date
    public async Task<List<Flight>> SearchAsync(string? source, string? destination, DateTime? date)
    {
        var now = DateTime.UtcNow;
        var query = _context.Flights.Where(f => f.DepartureTime > now).AsQueryable();

        if (!string.IsNullOrWhiteSpace(source))
            query = query.Where(f => f.Source.ToLower().Contains(source.ToLower()));

        if (!string.IsNullOrWhiteSpace(destination))
            query = query.Where(f => f.Destination.ToLower().Contains(destination.ToLower()));

        if (date.HasValue)
            query = query.Where(f => f.DepartureTime.Date == date.Value.Date);

        return await query.OrderBy(f => f.DepartureTime).ToListAsync();
    }

    // Create new flight
    public async Task<Flight> CreateAsync(Flight flight)
    {
        await _context.Flights.AddAsync(flight);
        await _context.SaveChangesAsync();
        return flight;
    }

    // Update existing flight
    public async Task<Flight> UpdateAsync(Flight flight)
    {
        _context.Flights.Update(flight);
        await _context.SaveChangesAsync();
        return flight;
    }

    // Delete flight by ID
    public async Task<bool> DeleteAsync(int id)
    {
        var flight = await _context.Flights.FindAsync(id);
        if (flight == null) return false;

        _context.Flights.Remove(flight);
        await _context.SaveChangesAsync();
        return true;
    }
}
