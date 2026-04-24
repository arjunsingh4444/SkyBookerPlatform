using Microsoft.EntityFrameworkCore;
using SkyBooker.AuthService.Data;
using SkyBooker.AuthService.Entities;
using SkyBooker.AuthService.Interfaces;

namespace SkyBooker.AuthService.Repository;

// This class talks to the database for User-related operations.
// It uses Entity Framework Core to run SQL queries behind the scenes.

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    // Get all users from database
    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    // Find a user by their ID
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    // Find a user by their email (case-insensitive)
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    // Save a new user to the database
    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // Update an existing user in the database
    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // Check if a user with this email already exists
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
}
