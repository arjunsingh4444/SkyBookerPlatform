using SkyBooker.PassengerService.Dtos;
using SkyBooker.PassengerService.Entities;
using SkyBooker.PassengerService.Interfaces;

namespace SkyBooker.PassengerService.Services;

public class PassengerServiceImpl : IPassengerService
{
    private readonly IPassengerRepository _repo;

    public PassengerServiceImpl(IPassengerRepository repo) { _repo = repo; }

    public async Task<ApiResponse<List<PassengerResponseDto>>> GetMyPassengersAsync(int userId)
    {
        var passengers = await _repo.GetByUserIdAsync(userId);
        return ApiResponse<List<PassengerResponseDto>>.Ok(passengers.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse<PassengerResponseDto>> GetPassengerByIdAsync(int id, int userId)
    {
        var passenger = await _repo.GetByIdAsync(id);
        if (passenger == null) return ApiResponse<PassengerResponseDto>.Fail("Passenger not found");
        
        if (passenger.UserId != userId)
            return ApiResponse<PassengerResponseDto>.Fail("Unauthorized access to this passenger");

        return ApiResponse<PassengerResponseDto>.Ok(MapToDto(passenger));
    }

    public async Task<ApiResponse<PassengerResponseDto>> CreatePassengerAsync(int userId, PassengerDto dto)
    {
        var passenger = new Passenger
        {
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            PassportNumber = dto.PassportNumber,
            Nationality = dto.Nationality
        };

        await _repo.CreateAsync(passenger);
        return ApiResponse<PassengerResponseDto>.Ok(MapToDto(passenger), "Passenger saved successfully");
    }

    public async Task<ApiResponse<PassengerResponseDto>> UpdatePassengerAsync(int id, int userId, PassengerDto dto)
    {
        var passenger = await _repo.GetByIdAsync(id);
        if (passenger == null) return ApiResponse<PassengerResponseDto>.Fail("Passenger not found");

        if (passenger.UserId != userId)
            return ApiResponse<PassengerResponseDto>.Fail("Unauthorized to update this passenger");

        passenger.FirstName = dto.FirstName;
        passenger.LastName = dto.LastName;
        passenger.Gender = dto.Gender;
        passenger.DateOfBirth = dto.DateOfBirth;
        passenger.PassportNumber = dto.PassportNumber;
        passenger.Nationality = dto.Nationality;

        await _repo.UpdateAsync(passenger);
        return ApiResponse<PassengerResponseDto>.Ok(MapToDto(passenger), "Passenger updated successfully");
    }

    public async Task<ApiResponse> DeletePassengerAsync(int id, int userId)
    {
        var passenger = await _repo.GetByIdAsync(id);
        if (passenger == null) return ApiResponse.Fail("Passenger not found");

        if (passenger.UserId != userId)
            return ApiResponse.Fail("Unauthorized to delete this passenger");

        await _repo.DeleteAsync(passenger);
        return ApiResponse.Ok("Passenger deleted successfully");
    }

    private static PassengerResponseDto MapToDto(Passenger p) => new()
    {
        Id = p.Id,
        UserId = p.UserId,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Gender = p.Gender,
        DateOfBirth = p.DateOfBirth,
        PassportNumber = p.PassportNumber,
        Nationality = p.Nationality,
        CreatedAt = p.CreatedAt
    };
}
