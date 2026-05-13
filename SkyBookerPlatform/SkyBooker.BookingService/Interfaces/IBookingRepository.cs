using SkyBooker.BookingService.Entities;

namespace SkyBooker.BookingService.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking?> GetByPNRAsync(string pnr);
    Task<List<Booking>> GetByUserIdAsync(int userId);
    Task<List<Booking>> GetAllAsync();
    Task<Booking> CreateAsync(Booking booking);
    Task<Booking> UpdateAsync(Booking booking);
}
