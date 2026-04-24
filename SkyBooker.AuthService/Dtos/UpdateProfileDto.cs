using System.ComponentModel.DataAnnotations;

namespace SkyBooker.AuthService.Dtos;

// Data sent when updating a user's profile
// Must include the user ID, and can update: fullName, email, phone

public class UpdateProfileDto
{
    [Required(ErrorMessage = "User ID is required")]
    public int Id { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
    public string? FullName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? Phone { get; set; }
}
