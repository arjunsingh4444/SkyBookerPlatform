using Microsoft.AspNetCore.Mvc;
using SkyBooker.NotificationService.Data;
using SkyBooker.NotificationService.Dtos;
using SkyBooker.NotificationService.Entities;

namespace SkyBooker.NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly NotificationDbContext _context;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(NotificationDbContext context, ILogger<NotificationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationDto dto)
    {
        // Simulate sending an email or SMS here
        _logger.LogInformation("Sending {Type} to {Recipient}: {Subject}", dto.Type, dto.Recipient, dto.Subject);

        // Save to DB
        var log = new NotificationLog
        {
            Recipient = dto.Recipient,
            Subject = dto.Subject,
            Message = dto.Message,
            Type = dto.Type,
            Status = "Sent",
            SentAt = DateTime.UtcNow
        };

        _context.NotificationLogs.Add(log);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.Ok($"Notification sent successfully to {dto.Recipient}"));
    }

    [HttpGet("history")]
    public IActionResult GetHistory()
    {
        var logs = _context.NotificationLogs.OrderByDescending(n => n.SentAt).Take(50).ToList();
        return Ok(ApiResponse<List<NotificationLog>>.Ok(logs, "Recent notifications fetched"));
    }
}
