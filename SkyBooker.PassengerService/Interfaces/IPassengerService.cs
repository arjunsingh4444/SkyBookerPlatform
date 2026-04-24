using SkyBooker.PassengerService.Dtos;

namespace SkyBooker.PassengerService.Interfaces;

public interface IPassengerService
{
    Task<ApiResponse<List<PassengerResponseDto>>> GetMyPassengersAsync(int userId);
    Task<ApiResponse<PassengerResponseDto>> GetPassengerByIdAsync(int id, int userId);
    Task<ApiResponse<PassengerResponseDto>> CreatePassengerAsync(int userId, PassengerDto dto);
    Task<ApiResponse<PassengerResponseDto>> UpdatePassengerAsync(int id, int userId, PassengerDto dto);
    Task<ApiResponse> DeletePassengerAsync(int id, int userId);
}
