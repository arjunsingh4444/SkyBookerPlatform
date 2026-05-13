using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyBooker.FlightService.Dtos;
using SkyBooker.FlightService.Interfaces;

namespace SkyBooker.FlightService.Controllers;

// Flight API endpoints
// Base URL: /api/Flight

[ApiController]
[Route("api/[controller]")]
public class FlightController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    // ──────────── GET /api/Flight ────────────
    // Get all flights
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _flightService.GetAllFlightsAsync();
        return Ok(result);
    }

    // ──────────── GET /api/Flight/{id} ────────────
    // Get a single flight by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _flightService.GetFlightByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    // ──────────── GET /api/Flight/search?source=Delhi&destination=Mumbai&date=2026-05-01 ────────────
    // Search flights by source, destination, and/or date
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? source,
        [FromQuery] string? destination,
        [FromQuery] DateTime? date)
    {
        var result = await _flightService.SearchFlightsAsync(source, destination, date);
        return Ok(result);
    }

    // ──────────── POST /api/Flight ────────────
    // Create a new flight (protected - needs JWT)
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateFlightDto dto)
    {
        var result = await _flightService.CreateFlightAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    // ──────────── PUT /api/Flight ────────────
    // Update an existing flight (protected - needs JWT)
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] UpdateFlightDto dto)
    {
        var result = await _flightService.UpdateFlightAsync(dto);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    // ──────────── DELETE /api/Flight/{id} ────────────
    // Delete a flight (protected - needs JWT)
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _flightService.DeleteFlightAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    // ──────────── POST /api/Flight/{id}/adjust-seats ────────────
    // Adjust available seats (called by BookingService)
    [HttpPost("{id}/adjust-seats")]
    public async Task<IActionResult> AdjustSeats(int id, [FromQuery] int adjustment)
    {
        var result = await _flightService.AdjustAvailableSeatsAsync(id, adjustment);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
