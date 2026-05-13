namespace SkyBooker.AuthService.Dtos;

// User profile data sent to the client (no password hash!)
// Used in: GET /api/Auth/me response, login response

public class UserProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
