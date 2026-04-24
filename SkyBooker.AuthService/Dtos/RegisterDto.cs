using System.ComponentModel.DataAnnotations;

namespace SkyBooker.AuthService.Dtos;

// Data sent by user when registering a new account
// Example: { "fullName": "Arjun", "email": "arjun@mail.com", "password": "123456", "phone": "9876543210" }

public class RegisterDto
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; } = string.Empty;
}
