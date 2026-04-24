using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyBooker.AuthService.Dtos;
using SkyBooker.AuthService.Interfaces;

namespace SkyBooker.AuthService.Controllers;

// This controller handles all Auth-related API endpoints.
// Base URL: /api/Auth

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // ──────────── POST /api/Auth/register ────────────
    // Anyone can call this (no login required)
    // Returns only a message (no token - user must login separately)
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (!result.Success)
            return BadRequest(result);      // 400 - something went wrong

        return Ok(result);                  // 200 - user created successfully
    }

    // ──────────── POST /api/Auth/login ────────────
    // Anyone can call this (no login required)
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (!result.Success)
            return Unauthorized(result);    // 401 - wrong email or password

        return Ok(result);                  // 200 - login successful, returns JWT token
    }

    // ──────────── GET /api/Auth/users ────────────
    // Returns all users in the database
    [HttpGet("users")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _authService.GetAllUsersAsync();
        return Ok(result);
    }

    // ──────────── GET /api/Auth/{id} ────────────
    // Returns a single user by their ID
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _authService.GetProfileAsync(id);

        if (!result.Success)
            return NotFound(result);        // 404 - user not found

        return Ok(result);                  // 200 - returns user info
    }

    // ──────────── PUT /api/Auth/update-profile ────────────
    // Pass user ID in the body along with fields to update
    [HttpPut("update-profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var result = await _authService.UpdateProfileAsync(dto);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);                  // 200 - profile updated
    }

    // ──────────── POST /api/Auth/logout ────────────
    // Protected: user must send JWT token in header
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // JWT is stateless, so we just return success.
        // The frontend should delete the token from storage.
        return Ok(ApiResponse.Ok("Logged out successfully"));
    }

    // ──────────── HELPER: Extract user ID from JWT token ────────────
    private int? GetUserIdFromToken()
    {
        // The JWT token contains claims. We read the "NameIdentifier" claim which has the user ID.
        string? userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Try to convert string to int
        if (string.IsNullOrEmpty(userIdString))
            return null;

        if (int.TryParse(userIdString, out int userId))
            return userId;

        return null;
    }
}
