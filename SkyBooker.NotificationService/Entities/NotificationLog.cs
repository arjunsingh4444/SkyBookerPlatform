namespace SkyBooker.NotificationService.Entities;

public class NotificationLog
{
    public int Id { get; set; }
    public string Recipient { get; set; } = string.Empty; // Email or Phone number
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Email";           // Email, SMS, Push
    public string Status { get; set; } = "Sent";          // Sent, Failed
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
