using SkyBooker.AuthService.Entities;

namespace SkyBooker.AuthService.Interfaces;

// Contract for database operations on Users
// The actual implementation is in Repository/UserRepository.cs

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> ExistsByEmailAsync(string email);
}
