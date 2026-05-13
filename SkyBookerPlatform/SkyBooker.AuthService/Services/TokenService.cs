using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SkyBooker.AuthService.Entities;
using SkyBooker.AuthService.Interfaces;

namespace SkyBooker.AuthService.Services;

// This service creates JWT tokens when a user logs in or registers.
// JWT = JSON Web Token → a secure string that proves "who the user is"

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Takes a User and returns a JWT token string
    public string GenerateToken(User user)
    {
        // Step 1: Read JWT settings from appsettings.json
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;

        // Step 2: Create the signing key (used to sign the token so no one can tamper with it)
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Step 3: Define what info to put inside the token (called "claims")
        // These claims are readable when we decode the token later
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // User's ID
            new Claim(ClaimTypes.Email, user.Email),                    // User's Email
            new Claim(ClaimTypes.Name, user.FullName),                  // User's Name
            new Claim(ClaimTypes.Role, user.Role.ToString())            // User's Role (User/Admin)
        };

        // Step 4: Set how long the token is valid (from appsettings, default = 60 minutes)
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        // Step 5: Build the token
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],          // Who created this token
            audience: jwtSettings["Audience"],      // Who this token is for
            claims: claims,                          // User info stored inside
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),  // When it expires
            signingCredentials: credentials          // Signature to verify it's real
        );

        // Step 6: Convert token object to a string and return it
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
