using SkyBooker.SeatService.Entities;

namespace SkyBooker.SeatService.Interfaces;

public interface ISeatRepository
{
    Task<List<Seat>> GetByFlightIdAsync(int flightId);
    Task<List<Seat>> GetAvailableByFlightIdAsync(int flightId);
    Task<Seat?> GetByIdAsync(int id);
    Task<Seat> CreateAsync(Seat seat);
    Task CreateManyAsync(List<Seat> seats);
    Task<Seat> UpdateAsync(Seat seat);
    Task<bool> ExistsAsync(int flightId, string seatNumber);
    Task<Seat?> GetByFlightAndSeatNumberAsync(int flightId, string seatNumber);
}
