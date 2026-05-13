using Microsoft.EntityFrameworkCore;
using SkyBooker.PassengerService.Data;
using SkyBooker.PassengerService.Entities;
using SkyBooker.PassengerService.Interfaces;

namespace SkyBooker.PassengerService.Repository;

public class PassengerRepository : IPassengerRepository
{
    private readonly PassengerDbContext _context;

    public PassengerRepository(PassengerDbContext context) { _context = context; }

    public async Task<List<Passenger>> GetByUserIdAsync(int userId)
        => await _context.Passengers.Where(p => p.UserId == userId).ToListAsync();

    public async Task<Passenger?> GetByIdAsync(int id)
        => await _context.Passengers.FindAsync(id);

    public async Task<Passenger> CreateAsync(Passenger passenger)
    {
        await _context.Passengers.AddAsync(passenger);
        await _context.SaveChangesAsync();
        return passenger;
    }

    public async Task<Passenger> UpdateAsync(Passenger passenger)
    {
        _context.Passengers.Update(passenger);
        await _context.SaveChangesAsync();
        return passenger;
    }

    public async Task DeleteAsync(Passenger passenger)
    {
        _context.Passengers.Remove(passenger);
        await _context.SaveChangesAsync();
    }
}
