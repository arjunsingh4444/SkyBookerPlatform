using System.ComponentModel.DataAnnotations;

namespace SkyBooker.NotificationService.Dtos;

public class SendNotificationDto
{
    [Required]
    public string Recipient { get; set; } = string.Empty; // Email or Phone

    public string Subject { get; set; } = "SkyBooker Notification";

    [Required]
    public string Message { get; set; } = string.Empty;

    public string Type { get; set; } = "Email"; // Email or SMS
}
