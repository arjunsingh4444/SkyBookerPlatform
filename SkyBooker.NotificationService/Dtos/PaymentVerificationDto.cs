namespace SkyBooker.NotificationService.Dtos;

public class PaymentVerificationDto
{
    public string PaymentId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
}
