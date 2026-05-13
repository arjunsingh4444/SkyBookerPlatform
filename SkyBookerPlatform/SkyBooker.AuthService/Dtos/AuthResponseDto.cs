namespace SkyBooker.AuthService.Dtos;

// Response sent after successful login
// Contains only the JWT token

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
}
