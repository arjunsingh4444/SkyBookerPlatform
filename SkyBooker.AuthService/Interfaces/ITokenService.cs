using SkyBooker.AuthService.Entities;

namespace SkyBooker.AuthService.Interfaces;

// Contract for JWT token generation
// The actual implementation is in Services/TokenService.cs

public interface ITokenService
{
    string GenerateToken(User user);  // Takes a user, returns a JWT token string
}
