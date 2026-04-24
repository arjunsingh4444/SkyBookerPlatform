using System.ComponentModel.DataAnnotations;

namespace SkyBooker.AuthService.Dtos;

// Data sent by user when logging in
// Example: { "email": "arjun@mail.com", "password": "123456" }

public class LoginDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}
