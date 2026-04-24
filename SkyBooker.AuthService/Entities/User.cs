namespace SkyBooker.AuthService.Entities;

// User entity → represents a row in the "Users" table

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;  // Hashed password (never plain text!)
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;       // Default role is "User"
}
