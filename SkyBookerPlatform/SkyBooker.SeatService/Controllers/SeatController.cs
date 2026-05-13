using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyBooker.SeatService.Dtos;
using SkyBooker.SeatService.Interfaces;

namespace SkyBooker.SeatService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeatController : ControllerBase
{
    private readonly ISeatService _seatService;

    public SeatController(ISeatService seatService) { _seatService = seatService; }

    // ──────────── GET /api/Seat/flight/{flightId} ────────────
    // Get all seats for a flight
    [HttpGet("flight/{flightId}")]
    public async Task<IActionResult> GetByFlight(int flightId)
    {
        var result = await _seatService.GetSeatsByFlightAsync(flightId);
        return Ok(result);
    }

    // ──────────── GET /api/Seat/flight/{flightId}/available ────────────
    // Get only available seats for a flight
    [HttpGet("flight/{flightId}/available")]
    public async Task<IActionResult> GetAvailable(int flightId)
    {
        var result = await _seatService.GetAvailableSeatsByFlightAsync(flightId);
        return Ok(result);
    }

    // ──────────── GET /api/Seat/{id} ────────────
    // Get a single seat by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _seatService.GetSeatByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    // ──────────── POST /api/Seat ────────────
    // Create a single seat (protected)
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateSeatDto dto)
    {
        var result = await _seatService.CreateSeatAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    // ──────────── POST /api/Seat/generate ────────────
    // Generate multiple seats for a flight (protected)
    [HttpPost("generate")]
    [Authorize]
    public async Task<IActionResult> Generate([FromBody] GenerateSeatsDto dto)
    {
        var result = await _seatService.GenerateSeatsAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    // ──────────── POST /api/Seat/lock ────────────
    // Lock a seat temporarily (protected)
    [HttpPost("lock")]
    [Authorize]
    public async Task<IActionResult> Lock([FromBody] SeatActionDto dto)
    {
        var result = await _seatService.LockSeatAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    // ──────────── POST /api/Seat/book ────────────
    // Book a seat permanently (protected)
    [HttpPost("book")]
    [Authorize]
    public async Task<IActionResult> Book([FromBody] SeatActionDto dto)
    {
        var result = await _seatService.BookSeatAsync(dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    // ──────────── PUT /api/Seat/{id} ────────────
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSeatDto dto)
    {
        var result = await _seatService.UpdateSeatAsync(id, dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    // ──────────── DELETE /api/Seat/{id} ────────────
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _seatService.DeleteSeatAsync(id);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
