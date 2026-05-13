using SkyBooker.AuthService.Dtos;

namespace SkyBooker.AuthService.Interfaces;

// Contract for authentication business logic
// The actual implementation is in Services/AuthServiceImpl.cs

public interface IAuthService
{
    Task<ApiResponse> RegisterAsync(RegisterDto dto);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<ApiResponse<List<UserProfileDto>>> GetAllUsersAsync();
    Task<ApiResponse<UserProfileDto>> GetProfileAsync(int userId);
    Task<ApiResponse<UserProfileDto>> UpdateProfileAsync(UpdateProfileDto dto);
}
