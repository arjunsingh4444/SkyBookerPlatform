using SkyBooker.BookingService.Dtos;
using SkyBooker.BookingService.Entities;
using SkyBooker.BookingService.Interfaces;

namespace SkyBooker.BookingService.Services;

public class BookingServiceImpl : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly ILogger<BookingServiceImpl> _logger;

    public BookingServiceImpl(IBookingRepository bookingRepo, ILogger<BookingServiceImpl> logger)
    { _bookingRepo = bookingRepo; _logger = logger; }

    public async Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(int userId, CreateBookingDto dto)
    {
        var booking = new Booking
        {
            PNR = GeneratePNR(),
            UserId = userId,
            FlightId = dto.FlightId,
            SeatNumber = dto.SeatNumber,
            TotalAmount = dto.TotalAmount,
            Status = BookingStatus.Confirmed,
            PassengerName = dto.PassengerName,
            PassengerEmail = dto.PassengerEmail,
            PassengerPhone = dto.PassengerPhone
        };

        await _bookingRepo.CreateAsync(booking);
        _logger.LogInformation("Booking {PNR} created by User {UserId}", booking.PNR, userId);

        return ApiResponse<BookingResponseDto>.Ok(MapToDto(booking), "Booking successful!");
    }

    public async Task<ApiResponse<BookingResponseDto>> GetBookingByIdAsync(int id, int userId, string role)
    {
        var booking = await _bookingRepo.GetByIdAsync(id);
        if (booking == null) return ApiResponse<BookingResponseDto>.Fail("Booking not found");

        if (role != "Admin" && booking.UserId != userId)
            return ApiResponse<BookingResponseDto>.Fail("Unauthorized access to this booking");

        return ApiResponse<BookingResponseDto>.Ok(MapToDto(booking));
    }

    public async Task<ApiResponse<BookingResponseDto>> GetBookingByPNRAsync(string pnr, int userId, string role)
    {
        var booking = await _bookingRepo.GetByPNRAsync(pnr.ToUpper());
        if (booking == null) return ApiResponse<BookingResponseDto>.Fail("Booking not found");

        if (role != "Admin" && booking.UserId != userId)
            return ApiResponse<BookingResponseDto>.Fail("Unauthorized access to this booking");

        return ApiResponse<BookingResponseDto>.Ok(MapToDto(booking));
    }

    public async Task<ApiResponse<List<BookingResponseDto>>> GetMyBookingsAsync(int userId)
    {
        var bookings = await _bookingRepo.GetByUserIdAsync(userId);
        return ApiResponse<List<BookingResponseDto>>.Ok(bookings.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse<List<BookingResponseDto>>> GetAllBookingsAsync()
    {
        var bookings = await _bookingRepo.GetAllAsync();
        return ApiResponse<List<BookingResponseDto>>.Ok(bookings.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse> CancelBookingAsync(int id, int userId, string role)
    {
        var booking = await _bookingRepo.GetByIdAsync(id);
        if (booking == null) return ApiResponse.Fail("Booking not found");

        if (role != "Admin" && booking.UserId != userId)
            return ApiResponse.Fail("Unauthorized to cancel this booking");

        if (booking.Status == BookingStatus.Cancelled)
            return ApiResponse.Fail("Booking is already cancelled");

        booking.Status = BookingStatus.Cancelled;
        await _bookingRepo.UpdateAsync(booking);

        _logger.LogInformation("Booking {PNR} cancelled by User {UserId}", booking.PNR, userId);
        return ApiResponse.Ok("Booking cancelled successfully");
    }

    private string GeneratePNR()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return "SKY-" + new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static BookingResponseDto MapToDto(Booking booking) => new()
    {
        Id = booking.Id,
        PNR = booking.PNR,
        UserId = booking.UserId,
        FlightId = booking.FlightId,
        SeatNumber = booking.SeatNumber,
        TotalAmount = booking.TotalAmount,
        Status = booking.Status.ToString(),
        PassengerName = booking.PassengerName,
        CreatedAt = booking.CreatedAt
    };
}
