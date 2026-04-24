using SkyBooker.FlightService.Dtos;

namespace SkyBooker.FlightService.Interfaces;

// Contract for flight business logic
public interface IFlightService
{
    Task<ApiResponse<List<FlightResponseDto>>> GetAllFlightsAsync();
    Task<ApiResponse<FlightResponseDto>> GetFlightByIdAsync(int id);
    Task<ApiResponse<List<FlightResponseDto>>> SearchFlightsAsync(string? source, string? destination, DateTime? date);
    Task<ApiResponse> CreateFlightAsync(CreateFlightDto dto);
    Task<ApiResponse<FlightResponseDto>> UpdateFlightAsync(UpdateFlightDto dto);
    Task<ApiResponse> DeleteFlightAsync(int id);
}
