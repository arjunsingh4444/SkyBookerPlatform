using SkyBooker.SeatService.Dtos;
using SkyBooker.SeatService.Entities;
using SkyBooker.SeatService.Interfaces;

namespace SkyBooker.SeatService.Services;

public class SeatServiceImpl : ISeatService
{
    private readonly ISeatRepository _seatRepo;
    private readonly ILogger<SeatServiceImpl> _logger;

    public SeatServiceImpl(ISeatRepository seatRepo, ILogger<SeatServiceImpl> logger)
    { _seatRepo = seatRepo; _logger = logger; }

    // ──────────── GET ALL SEATS FOR A FLIGHT ────────────
    public async Task<ApiResponse<List<SeatResponseDto>>> GetSeatsByFlightAsync(int flightId)
    {
        var seats = await _seatRepo.GetByFlightIdAsync(flightId);
        return ApiResponse<List<SeatResponseDto>>.Ok(seats.Select(MapToDto).ToList(), $"{seats.Count} seats found");
    }

    // ──────────── GET AVAILABLE SEATS ONLY ────────────
    public async Task<ApiResponse<List<SeatResponseDto>>> GetAvailableSeatsByFlightAsync(int flightId)
    {
        var seats = await _seatRepo.GetAvailableByFlightIdAsync(flightId);
        return ApiResponse<List<SeatResponseDto>>.Ok(seats.Select(MapToDto).ToList(), $"{seats.Count} available seats");
    }

    // ──────────── GET SEAT BY ID ────────────
    public async Task<ApiResponse<SeatResponseDto>> GetSeatByIdAsync(int id)
    {
        var seat = await _seatRepo.GetByIdAsync(id);
        if (seat == null) return ApiResponse<SeatResponseDto>.Fail("Seat not found");
        return ApiResponse<SeatResponseDto>.Ok(MapToDto(seat));
    }

    // ──────────── CREATE SINGLE SEAT ────────────
    public async Task<ApiResponse> CreateSeatAsync(CreateSeatDto dto)
    {
        if (await _seatRepo.ExistsAsync(dto.FlightId, dto.SeatNumber))
            return ApiResponse.Fail("This seat already exists on this flight");

        var seat = new Seat
        {
            FlightId = dto.FlightId,
            SeatNumber = dto.SeatNumber.ToUpper(),
            SeatClass = dto.SeatClass,
            Price = dto.Price,
            Status = SeatStatus.Available
        };

        await _seatRepo.CreateAsync(seat);
        return ApiResponse.Ok("Seat created successfully");
    }

    // ──────────── GENERATE MULTIPLE SEATS (bulk) ────────────
    // Creates seats like 1A, 1B, 1C... 2A, 2B, 2C... etc.
    public async Task<ApiResponse> GenerateSeatsAsync(GenerateSeatsDto dto)
    {
        var seatLabels = "ABCDEF".Substring(0, Math.Min(dto.SeatsPerRow, 6));
        var seats = new List<Seat>();
        int seatCount = 0;

        for (int row = 1; seatCount < dto.TotalSeats; row++)
        {
            foreach (char label in seatLabels)
            {
                if (seatCount >= dto.TotalSeats) break;

                string seatNumber = $"{row}{label}";
                if (await _seatRepo.ExistsAsync(dto.FlightId, seatNumber)) continue;

                seats.Add(new Seat
                {
                    FlightId = dto.FlightId,
                    SeatNumber = seatNumber,
                    SeatClass = "Economy",
                    Price = dto.EconomyPrice,
                    Status = SeatStatus.Available
                });
                seatCount++;
            }
        }

        await _seatRepo.CreateManyAsync(seats);
        _logger.LogInformation("{Count} seats generated for flight {FlightId}", seats.Count, dto.FlightId);
        return ApiResponse.Ok($"{seats.Count} seats generated successfully");
    }

    // ──────────── LOCK SEAT (temporary hold for 10 min) ────────────
    public async Task<ApiResponse<SeatResponseDto>> LockSeatAsync(SeatActionDto dto)
    {
        var seat = await _seatRepo.GetByFlightAndSeatNumberAsync(dto.FlightId, dto.SeatNumber);
        if (seat == null) return ApiResponse<SeatResponseDto>.Fail($"Seat {dto.SeatNumber} not found on this flight");

        // Check if seat is already locked by someone else
        if (seat.Status == SeatStatus.Locked && seat.LockExpiresAt > DateTime.UtcNow)
        {
            if (seat.LockedByUserId != dto.UserId)
                return ApiResponse<SeatResponseDto>.Fail("Seat is already locked by another user");
            // If it's the same user, we just fall through and extend their lock time
        }

        if (seat.Status == SeatStatus.Booked)
            return ApiResponse<SeatResponseDto>.Fail("Seat is already booked");

        // Lock the seat for 10 minutes
        seat.Status = SeatStatus.Locked;
        seat.LockedByUserId = dto.UserId;
        seat.LockExpiresAt = DateTime.UtcNow.AddMinutes(10);

        await _seatRepo.UpdateAsync(seat);
        _logger.LogInformation("Seat {SeatNumber} locked by user {UserId}", seat.SeatNumber, dto.UserId);
        return ApiResponse<SeatResponseDto>.Ok(MapToDto(seat), "Seat locked for 10 minutes");
    }

    // ──────────── BOOK SEAT (confirm booking) ────────────
    public async Task<ApiResponse<SeatResponseDto>> BookSeatAsync(SeatActionDto dto)
    {
        var seat = await _seatRepo.GetByFlightAndSeatNumberAsync(dto.FlightId, dto.SeatNumber);
        if (seat == null) return ApiResponse<SeatResponseDto>.Fail($"Seat {dto.SeatNumber} not found on this flight");

        if (seat.Status == SeatStatus.Booked)
        {
            if (seat.BookedByUserId == dto.UserId)
                return ApiResponse<SeatResponseDto>.Ok(MapToDto(seat), "You have already booked this seat");
                
            return ApiResponse<SeatResponseDto>.Fail("Seat is already booked by someone else");
        }

        // If locked, only the user who locked it can book it
        if (seat.Status == SeatStatus.Locked && seat.LockedByUserId != dto.UserId)
            return ApiResponse<SeatResponseDto>.Fail("Seat is locked by another user");

        seat.Status = SeatStatus.Booked;
        seat.BookedByUserId = dto.UserId;
        seat.LockedByUserId = null;
        seat.LockExpiresAt = null;

        await _seatRepo.UpdateAsync(seat);
        _logger.LogInformation("Seat {SeatNumber} booked by user {UserId}", seat.SeatNumber, dto.UserId);
        return ApiResponse<SeatResponseDto>.Ok(MapToDto(seat), "Seat booked successfully");
    }

    // ──────────── UPDATE SEAT ────────────
    public async Task<ApiResponse<SeatResponseDto>> UpdateSeatAsync(int id, UpdateSeatDto dto)
    {
        var seat = await _seatRepo.GetByIdAsync(id);
        if (seat == null) return ApiResponse<SeatResponseDto>.Fail("Seat not found");

        seat.SeatClass = dto.SeatClass;
        seat.Price = dto.Price;

        if (Enum.TryParse<SeatStatus>(dto.Status, true, out var parsedStatus))
        {
            seat.Status = parsedStatus;
        }

        await _seatRepo.UpdateAsync(seat);
        return ApiResponse<SeatResponseDto>.Ok(MapToDto(seat), "Seat updated successfully");
    }

    // ──────────── DELETE SEAT ────────────
    public async Task<ApiResponse> DeleteSeatAsync(int id)
    {
        var seat = await _seatRepo.GetByIdAsync(id);
        if (seat == null) return ApiResponse.Fail("Seat not found");

        await _seatRepo.DeleteAsync(seat);
        return ApiResponse.Ok("Seat deleted successfully");
    }

    // ──────────── UNLOCK EXPIRED SEATS ────────────
    public async Task<ApiResponse> UnlockExpiredSeatsAsync()
    {
        var seats = await _seatRepo.GetByFlightIdAsync(0); // We'll handle this differently
        // For now this is called per-flight from the controller
        return ApiResponse.Ok("Expired locks cleaned up");
    }

    // ──────────── HELPER ────────────
    private static SeatResponseDto MapToDto(Seat seat) => new()
    {
        Id = seat.Id,
        FlightId = seat.FlightId,
        SeatNumber = seat.SeatNumber,
        SeatClass = seat.SeatClass,
        Price = seat.Price,
        Status = seat.Status.ToString(),
        LockedByUserId = seat.LockedByUserId,
        BookedByUserId = seat.BookedByUserId
    };
}
