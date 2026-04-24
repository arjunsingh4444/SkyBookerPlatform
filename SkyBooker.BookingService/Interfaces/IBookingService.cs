using SkyBooker.BookingService.Dtos;

namespace SkyBooker.BookingService.Interfaces;

public interface IBookingService
{
    Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(int userId, CreateBookingDto dto);
    Task<ApiResponse<BookingResponseDto>> GetBookingByIdAsync(int id, int userId, string role);
    Task<ApiResponse<BookingResponseDto>> GetBookingByPNRAsync(string pnr, int userId, string role);
    Task<ApiResponse<List<BookingResponseDto>>> GetMyBookingsAsync(int userId);
    Task<ApiResponse<List<BookingResponseDto>>> GetAllBookingsAsync(); // Admin only
    Task<ApiResponse> CancelBookingAsync(int id, int userId, string role);
}
