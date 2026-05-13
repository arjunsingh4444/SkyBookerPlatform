using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyBooker.BookingService.Dtos;
using SkyBooker.BookingService.Interfaces;

namespace SkyBooker.BookingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All booking endpoints require authentication
public class BookingController : ControllerBase 
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService) { _bookingService = bookingService; }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string GetUserRole() => User.FindFirstValue(ClaimTypes.Role) ?? "User";

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var result = await _bookingService.CreateBookingAsync(GetUserId(), dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetMyBookings()
    {
        var result = await _bookingService.GetMyBookingsAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _bookingService.GetBookingByIdAsync(id, GetUserId(), GetUserRole());
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpGet("pnr/{pnr}")]
    public async Task<IActionResult> GetByPNR(string pnr)
    {
        var result = await _bookingService.GetBookingByPNRAsync(pnr, GetUserId(), GetUserRole());
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var result = await _bookingService.CancelBookingAsync(id, GetUserId(), GetUserRole());
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto dto)
    {
        var result = await _bookingService.UpdateBookingAsync(id, GetUserId(), GetUserRole(), dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    // Admin Only
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllBookings()
    {
        var result = await _bookingService.GetAllBookingsAsync();
        return Ok(result);
    }
}
