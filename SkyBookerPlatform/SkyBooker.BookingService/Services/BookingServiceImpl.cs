using System.Net.Http.Json;
using System.Text.Json;
using SkyBooker.BookingService.Dtos;
using SkyBooker.BookingService.Entities;
using SkyBooker.BookingService.Interfaces;

namespace SkyBooker.BookingService.Services;

public class BookingServiceImpl : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly ILogger<BookingServiceImpl> _logger;
    private readonly HttpClient _httpClient;

    public BookingServiceImpl(IBookingRepository bookingRepo, ILogger<BookingServiceImpl> logger, IHttpClientFactory httpClientFactory)
    { 
        _bookingRepo = bookingRepo; 
        _logger = logger; 
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(int userId, CreateBookingDto dto)
    {
        var bookingStatus = BookingStatus.Confirmed;
        if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<BookingStatus>(dto.Status, true, out var parsedStatus))
        {
            bookingStatus = parsedStatus;
        }

        var booking = new Booking
        {
            PNR = GeneratePNR(),
            UserId = userId,
            FlightId = dto.FlightId,
            SeatNumber = dto.SeatNumber,
            TotalAmount = dto.TotalAmount,
            Status = bookingStatus,
            PassengerName = dto.PassengerName,
            PassengerEmail = dto.PassengerEmail,
            PassengerPhone = dto.PassengerPhone
        };

        await _bookingRepo.CreateAsync(booking);
        _logger.LogInformation("Booking {PNR} created by User {UserId}", booking.PNR, userId);

        // ──────────── SYNC WITH OTHER SERVICES ────────────
        try 
        {
            // 1. Notify FlightService to decrement available seats
            var flightRes = await _httpClient.PostAsync($"http://localhost:5002/api/Flight/{dto.FlightId}/adjust-seats?adjustment=-1", null);
            if (!flightRes.IsSuccessStatusCode)
                _logger.LogWarning("Failed to update available seats in FlightService for flight {FlightId}", dto.FlightId);

            // 2. Notify SeatService to mark seat as Booked
            var seatPayload = new { flightId = dto.FlightId, seatNumber = dto.SeatNumber, userId = userId };
            var seatRes = await _httpClient.PostAsJsonAsync("http://localhost:5003/api/Seat/book", seatPayload);
            if (!seatRes.IsSuccessStatusCode)
                _logger.LogWarning("Failed to mark seat {SeatNumber} as Booked in SeatService", dto.SeatNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing with other services during booking creation");
        }

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

        if (role != "Admin" && booking.Status == BookingStatus.Confirmed)
            return ApiResponse.Fail("Sorry, confirmed bookings cannot be cancelled directly. Please contact support.");

        booking.Status = BookingStatus.Cancelled;
        await _bookingRepo.UpdateAsync(booking);

        _logger.LogInformation("Booking {PNR} cancelled by User {UserId}", booking.PNR, userId);

        // ──────────── SYNC WITH OTHER SERVICES ────────────
        try 
        {
            // 1. Notify FlightService to increment available seats back
            await _httpClient.PostAsync($"http://localhost:5002/api/Flight/{booking.FlightId}/adjust-seats?adjustment=1", null);
            
            // 2. Notify SeatService to mark seat as Available again
            var seatRes = await _httpClient.GetAsync($"http://localhost:5003/api/Seat/flight/{booking.FlightId}");
            if (seatRes.IsSuccessStatusCode)
            {
                var response = await seatRes.Content.ReadFromJsonAsync<JsonElement>();
                var seatsData = response.GetProperty("data");
                
                foreach (var s in seatsData.EnumerateArray())
                {
                    if (s.GetProperty("seatNumber").GetString() == booking.SeatNumber)
                    {
                        int seatId = s.GetProperty("id").GetInt32();
                        string seatClass = s.GetProperty("seatClass").GetString() ?? "Economy";
                        decimal price = s.GetProperty("price").GetDecimal();
                        await _httpClient.PutAsJsonAsync($"http://localhost:5003/api/Seat/{seatId}", new { status = "Available", seatClass = seatClass, price = price });
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing with other services during booking cancellation");
        }

        return ApiResponse.Ok("Booking cancelled successfully");
    }

    public async Task<ApiResponse> UpdateBookingAsync(int id, int userId, string role, UpdateBookingDto dto)
    {
        var booking = await _bookingRepo.GetByIdAsync(id);
        if (booking == null) return ApiResponse.Fail("Booking not found");

        if (role != "Admin" && booking.UserId != userId)
            return ApiResponse.Fail("Unauthorized to update this booking");

        if (role != "Admin" && booking.Status != BookingStatus.Pending)
            return ApiResponse.Fail("Only pending bookings can be edited.");

        // 1. Update Passenger Name
        if (!string.IsNullOrWhiteSpace(dto.PassengerName))
            booking.PassengerName = dto.PassengerName;

        // 2. Update Seat if changed
        if (!string.IsNullOrWhiteSpace(dto.SeatNumber) && dto.SeatNumber != booking.SeatNumber)
        {
            var oldSeat = booking.SeatNumber;
            var newSeat = dto.SeatNumber;

            try 
            {
                // Release old seat in SeatService
                var seatRes = await _httpClient.GetAsync($"http://localhost:5003/api/Seat/flight/{booking.FlightId}");
                if (seatRes.IsSuccessStatusCode)
                {
                    var response = await seatRes.Content.ReadFromJsonAsync<JsonElement>();
                    var seatsData = response.GetProperty("data");
                    
                    JsonElement? oldSeatElement = null;
                    foreach (var s in seatsData.EnumerateArray())
                    {
                        if (s.GetProperty("seatNumber").GetString() == oldSeat)
                        {
                            oldSeatElement = s;
                            break;
                        }
                    }

                    if (oldSeatElement.HasValue)
                    {
                        var s = oldSeatElement.Value;
                        int seatId = s.GetProperty("id").GetInt32();
                        string seatClass = s.GetProperty("seatClass").GetString() ?? "Economy";
                        decimal price = s.GetProperty("price").GetDecimal();

                        await _httpClient.PutAsJsonAsync($"http://localhost:5003/api/Seat/{seatId}", new { status = "Available", seatClass = seatClass, price = price });
                    }

                    // Book new seat
                    var bookRes = await _httpClient.PostAsJsonAsync("http://localhost:5003/api/Seat/book", new { flightId = booking.FlightId, seatNumber = newSeat, userId = userId });
                    if (bookRes.IsSuccessStatusCode)
                    {
                        booking.SeatNumber = newSeat;
                    }
                    else
                    {
                        var failRes = await bookRes.Content.ReadFromJsonAsync<ApiResponse>();
                        return ApiResponse.Fail(failRes?.Message ?? "Failed to book the new selected seat. It might be taken.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing seats during booking update");
                return ApiResponse.Fail("Communication error with Seat Service. Detail: " + ex.Message);
            }
        }

        // 3. Update Status
        if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<BookingStatus>(dto.Status, true, out var parsedStatus))
        {
            booking.Status = parsedStatus;
        }

        await _bookingRepo.UpdateAsync(booking);
        _logger.LogInformation("Booking {PNR} updated by User {UserId}", booking.PNR, userId);

        return ApiResponse.Ok("Booking updated successfully");
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
        PassengerEmail = booking.PassengerEmail,
        PassengerPhone = booking.PassengerPhone,
        CreatedAt = booking.CreatedAt
    };
}
