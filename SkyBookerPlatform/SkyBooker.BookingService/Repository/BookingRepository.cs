using Microsoft.EntityFrameworkCore;
using SkyBooker.BookingService.Data;
using SkyBooker.BookingService.Entities;
using SkyBooker.BookingService.Interfaces;

namespace SkyBooker.BookingService.Repository;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _context;

    public BookingRepository(BookingDbContext context) { _context = context; }

    public async Task<Booking?> GetByIdAsync(int id)
        => await _context.Bookings.FindAsync(id);

    public async Task<Booking?> GetByPNRAsync(string pnr)
        => await _context.Bookings.FirstOrDefaultAsync(b => b.PNR == pnr);

    public async Task<List<Booking>> GetByUserIdAsync(int userId)
        => await _context.Bookings.Where(b => b.UserId == userId).OrderByDescending(b => b.CreatedAt).ToListAsync();

    public async Task<List<Booking>> GetAllAsync()
        => await _context.Bookings.OrderByDescending(b => b.CreatedAt).ToListAsync();

    public async Task<Booking> CreateAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<Booking> UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
        return booking;
    }
}
