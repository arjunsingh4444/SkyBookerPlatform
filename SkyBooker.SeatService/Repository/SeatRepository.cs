using Microsoft.EntityFrameworkCore;
using SkyBooker.SeatService.Data;
using SkyBooker.SeatService.Entities;
using SkyBooker.SeatService.Interfaces;

namespace SkyBooker.SeatService.Repository;

public class SeatRepository : ISeatRepository
{
    private readonly SeatDbContext _context;

    public SeatRepository(SeatDbContext context) { _context = context; }

    public async Task<List<Seat>> GetByFlightIdAsync(int flightId)
        => await _context.Seats.Where(s => s.FlightId == flightId).OrderBy(s => s.SeatNumber).ToListAsync();

    public async Task<List<Seat>> GetAvailableByFlightIdAsync(int flightId)
        => await _context.Seats.Where(s => s.FlightId == flightId && s.Status == SeatStatus.Available).OrderBy(s => s.SeatNumber).ToListAsync();

    public async Task<Seat?> GetByIdAsync(int id)
        => await _context.Seats.FindAsync(id);

    public async Task<Seat> CreateAsync(Seat seat)
    {
        await _context.Seats.AddAsync(seat);
        await _context.SaveChangesAsync();
        return seat;
    }

    public async Task CreateManyAsync(List<Seat> seats)
    {
        await _context.Seats.AddRangeAsync(seats);
        await _context.SaveChangesAsync();
    }

    public async Task<Seat> UpdateAsync(Seat seat)
    {
        _context.Seats.Update(seat);
        await _context.SaveChangesAsync();
        return seat;
    }

    public async Task<bool> ExistsAsync(int flightId, string seatNumber)
        => await _context.Seats.AnyAsync(s => s.FlightId == flightId && s.SeatNumber == seatNumber);

    public async Task<Seat?> GetByFlightAndSeatNumberAsync(int flightId, string seatNumber)
        => await _context.Seats.FirstOrDefaultAsync(s => s.FlightId == flightId && s.SeatNumber.ToUpper() == seatNumber.ToUpper());
}
