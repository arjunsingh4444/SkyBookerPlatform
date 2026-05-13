using SkyBooker.FlightService.Dtos;
using SkyBooker.FlightService.Entities;
using SkyBooker.FlightService.Interfaces;

namespace SkyBooker.FlightService.Services;

// Business logic for Flight operations

public class FlightServiceImpl : IFlightService
{
    private readonly IFlightRepository _flightRepository;
    private readonly ILogger<FlightServiceImpl> _logger;

    public FlightServiceImpl(IFlightRepository flightRepository, ILogger<FlightServiceImpl> logger)
    {
        _flightRepository = flightRepository;
        _logger = logger;
    }

    // ──────────── GET ALL FLIGHTS ────────────
    public async Task<ApiResponse<List<FlightResponseDto>>> GetAllFlightsAsync()
    {
        var flights = await _flightRepository.GetAllAsync();
        var list = flights.Select(f => MapToDto(f)).ToList();
        return ApiResponse<List<FlightResponseDto>>.Ok(list, "All flights fetched successfully");
    }

    // ──────────── GET FLIGHT BY ID ────────────
    public async Task<ApiResponse<FlightResponseDto>> GetFlightByIdAsync(int id)
    {
        var flight = await _flightRepository.GetByIdAsync(id);
        if (flight == null)
            return ApiResponse<FlightResponseDto>.Fail("Flight not found");

        return ApiResponse<FlightResponseDto>.Ok(MapToDto(flight));
    }

    // ──────────── SEARCH FLIGHTS ────────────
    public async Task<ApiResponse<List<FlightResponseDto>>> SearchFlightsAsync(
        string? source, string? destination, DateTime? date)
    {
        var flights = await _flightRepository.SearchAsync(source, destination, date);
        var list = flights.Select(f => MapToDto(f)).ToList();
        return ApiResponse<List<FlightResponseDto>>.Ok(list, $"{list.Count} flights found");
    }

    // ──────────── CREATE FLIGHT ────────────
    public async Task<ApiResponse> CreateFlightAsync(CreateFlightDto dto)
    {
        // Check if flight number already exists
        var existing = await _flightRepository.GetByFlightNumberAsync(dto.FlightNumber);
        if (existing != null)
            return ApiResponse.Fail("A flight with this number already exists");

        // Create flight entity
        var flight = new Flight
        {
            FlightNumber = dto.FlightNumber.ToUpper(),
            AirlineName = dto.AirlineName,
            Source = dto.Source,
            Destination = dto.Destination,
            DepartureTime = dto.DepartureTime,
            ArrivalTime = dto.ArrivalTime,
            Price = dto.Price,
            TotalSeats = dto.TotalSeats,
            AvailableSeats = dto.TotalSeats,  // Initially all seats are available
            Status = FlightStatus.Scheduled
        };

        await _flightRepository.CreateAsync(flight);
        _logger.LogInformation("Flight created: {FlightNumber}", flight.FlightNumber);

        return ApiResponse.Ok("Flight created successfully");
    }

    // ──────────── UPDATE FLIGHT ────────────
    public async Task<ApiResponse<FlightResponseDto>> UpdateFlightAsync(UpdateFlightDto dto)
    {
        var flight = await _flightRepository.GetByIdAsync(dto.Id);
        if (flight == null)
            return ApiResponse<FlightResponseDto>.Fail("Flight not found");

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(dto.FlightNumber))
            flight.FlightNumber = dto.FlightNumber.ToUpper();

        if (!string.IsNullOrWhiteSpace(dto.AirlineName))
            flight.AirlineName = dto.AirlineName;

        if (!string.IsNullOrWhiteSpace(dto.Source))
            flight.Source = dto.Source;

        if (!string.IsNullOrWhiteSpace(dto.Destination))
            flight.Destination = dto.Destination;

        if (dto.DepartureTime.HasValue)
            flight.DepartureTime = dto.DepartureTime.Value;

        if (dto.ArrivalTime.HasValue)
            flight.ArrivalTime = dto.ArrivalTime.Value;

        if (dto.Price.HasValue)
            flight.Price = dto.Price.Value;

        if (dto.TotalSeats.HasValue)
            flight.TotalSeats = dto.TotalSeats.Value;

        if (dto.AvailableSeats.HasValue)
            flight.AvailableSeats = dto.AvailableSeats.Value;

        if (!string.IsNullOrWhiteSpace(dto.Status) && Enum.TryParse<FlightStatus>(dto.Status, true, out var status))
            flight.Status = status;

        await _flightRepository.UpdateAsync(flight);
        _logger.LogInformation("Flight updated: {FlightNumber}", flight.FlightNumber);

        return ApiResponse<FlightResponseDto>.Ok(MapToDto(flight), "Flight updated successfully");
    }

    // ──────────── DELETE FLIGHT ────────────
    public async Task<ApiResponse> DeleteFlightAsync(int id)
    {
        bool deleted = await _flightRepository.DeleteAsync(id);
        if (!deleted)
            return ApiResponse.Fail("Flight not found");

        _logger.LogInformation("Flight deleted: ID {Id}", id);
        return ApiResponse.Ok("Flight deleted successfully");
    }

    // ──────────── ADJUST AVAILABLE SEATS ────────────
    public async Task<ApiResponse> AdjustAvailableSeatsAsync(int id, int adjustment)
    {
        var flight = await _flightRepository.GetByIdAsync(id);
        if (flight == null) return ApiResponse.Fail("Flight not found");

        flight.AvailableSeats += adjustment;
        
        // Ensure available seats don't go below 0 or above total seats
        if (flight.AvailableSeats < 0) flight.AvailableSeats = 0;
        if (flight.AvailableSeats > flight.TotalSeats) flight.AvailableSeats = flight.TotalSeats;

        await _flightRepository.UpdateAsync(flight);
        _logger.LogInformation("Adjusted seats for flight {Id} by {Adjustment}. New available: {Available}", id, adjustment, flight.AvailableSeats);
        
        return ApiResponse.Ok("Seat count adjusted successfully");
    }

    // ──────────── HELPER: Convert Flight entity → FlightResponseDto ────────────
    private static FlightResponseDto MapToDto(Flight flight)
    {
        return new FlightResponseDto
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            AirlineName = flight.AirlineName,
            Source = flight.Source,
            Destination = flight.Destination,
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            Price = flight.Price,
            TotalSeats = flight.TotalSeats,
            AvailableSeats = flight.AvailableSeats,
            Status = flight.Status.ToString(),
            CreatedAt = flight.CreatedAt
        };
    }
}
