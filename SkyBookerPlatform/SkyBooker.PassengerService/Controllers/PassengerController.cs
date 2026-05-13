using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyBooker.PassengerService.Dtos;
using SkyBooker.PassengerService.Interfaces;

namespace SkyBooker.PassengerService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class PassengerController : ControllerBase
{
    private readonly IPassengerService _passengerService;

    public PassengerController(IPassengerService passengerService) { _passengerService = passengerService; }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetMyPassengers()
    {
        var result = await _passengerService.GetMyPassengersAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _passengerService.GetPassengerByIdAsync(id, GetUserId());
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PassengerDto dto)
    {
        var result = await _passengerService.CreatePassengerAsync(GetUserId(), dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PassengerDto dto)
    {
        var result = await _passengerService.UpdatePassengerAsync(id, GetUserId(), dto);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _passengerService.DeletePassengerAsync(id, GetUserId());
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
