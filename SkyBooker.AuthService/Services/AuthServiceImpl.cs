using SkyBooker.AuthService.Dtos;
using SkyBooker.AuthService.Entities;
using SkyBooker.AuthService.Interfaces;

namespace SkyBooker.AuthService.Services;

// This is the main brain of Auth Service.
// It handles: Register, Login, Get Profile, Update Profile

public class AuthServiceImpl : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthServiceImpl> _logger;

    public AuthServiceImpl(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<AuthServiceImpl> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    // ──────────── REGISTER ────────────
    public async Task<ApiResponse> RegisterAsync(RegisterDto dto)
    {
        // Step 1: Check if email is already taken
        bool emailExists = await _userRepository.ExistsByEmailAsync(dto.Email);
        if (emailExists)
        {
            return ApiResponse.Fail("A user with this email already exists");
        }

        // Step 2: Create a new user with hashed password
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),  // Never store plain password!
            Phone = dto.Phone,
            Role = UserRole.User  // New users are always "User" role
        };

        // Step 3: Save user to database
        await _userRepository.CreateAsync(user);
        _logger.LogInformation("User registered successfully: {Email}", user.Email);

        // Step 4: Return success message (user must login separately to get token)
        return ApiResponse.Ok("User registered successfully");
    }

    // ──────────── LOGIN ────────────
    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        // Step 1: Find user by email
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null)
        {
            return ApiResponse<AuthResponseDto>.Fail("Invalid email or password");
        }

        // Step 2: Check if password matches the stored hash
        bool passwordMatch = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!passwordMatch)
        {
            return ApiResponse<AuthResponseDto>.Fail("Invalid email or password");
        }

        _logger.LogInformation("User logged in successfully: {Email}", user.Email);

        // Step 3: Generate JWT token
        string token = _tokenService.GenerateToken(user);

        // Step 4: Return token only
        var response = new AuthResponseDto
        {
            Token = token
        };

        return ApiResponse<AuthResponseDto>.Ok(response, "Login successful");
    }

    // ──────────── GET ALL USERS ────────────
    public async Task<ApiResponse<List<UserProfileDto>>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        // Convert each user entity to profile DTO
        var userList = users.Select(u => MapToProfileDto(u)).ToList();

        return ApiResponse<List<UserProfileDto>>.Ok(userList, "All users fetched successfully");
    }

    // ──────────── GET USER BY ID ────────────
    public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(int userId)
    {
        // Find user by ID
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse<UserProfileDto>.Fail("User not found");
        }

        return ApiResponse<UserProfileDto>.Ok(MapToProfileDto(user));
    }

    // ──────────── UPDATE PROFILE ────────────
    public async Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(UpdateProfileDto dto)
    {
        // Step 1: Find user by the ID provided in the request
        var user = await _userRepository.GetByIdAsync(dto.Id);
        if (user == null)
        {
            return ApiResponse<UserProfileDto>.Fail("User not found");
        }

        // Step 2: Update only the fields that were provided
        if (!string.IsNullOrWhiteSpace(dto.FullName))
            user.FullName = dto.FullName;

        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email.ToLower();

        if (!string.IsNullOrWhiteSpace(dto.Phone))
            user.Phone = dto.Phone;

        // Step 3: Save changes to database
        await _userRepository.UpdateAsync(user);
        _logger.LogInformation("Profile updated for user: {Email}", user.Email);

        return ApiResponse<UserProfileDto>.Ok(MapToProfileDto(user), "Profile updated successfully");
    }

    // ──────────── HELPER: Convert User entity → UserProfileDto ────────────
    // This removes sensitive data (like password hash) before sending to client
    private static UserProfileDto MapToProfileDto(User user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
