using SkyBooker.PassengerService.Entities;

namespace SkyBooker.PassengerService.Interfaces;

public interface IPassengerRepository
{
    Task<List<Passenger>> GetByUserIdAsync(int userId);
    Task<Passenger?> GetByIdAsync(int id);
    Task<Passenger> CreateAsync(Passenger passenger);
    Task<Passenger> UpdateAsync(Passenger passenger);
    Task DeleteAsync(Passenger passenger);
}
