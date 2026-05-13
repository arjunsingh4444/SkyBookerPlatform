using SkyBooker.SeatService.Dtos;

namespace SkyBooker.SeatService.Interfaces;

public interface ISeatService
{
    Task<ApiResponse<List<SeatResponseDto>>> GetSeatsByFlightAsync(int flightId);
    Task<ApiResponse<List<SeatResponseDto>>> GetAvailableSeatsByFlightAsync(int flightId);
    Task<ApiResponse<SeatResponseDto>> GetSeatByIdAsync(int id);
    Task<ApiResponse> CreateSeatAsync(CreateSeatDto dto);
    Task<ApiResponse> GenerateSeatsAsync(GenerateSeatsDto dto);
    Task<ApiResponse<SeatResponseDto>> LockSeatAsync(SeatActionDto dto);
    Task<ApiResponse<SeatResponseDto>> BookSeatAsync(SeatActionDto dto);
    Task<ApiResponse<SeatResponseDto>> UpdateSeatAsync(int id, UpdateSeatDto dto);
    Task<ApiResponse> DeleteSeatAsync(int id);
    Task<ApiResponse> UnlockExpiredSeatsAsync();
}
