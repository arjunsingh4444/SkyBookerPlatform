namespace SkyBooker.NotificationService.Dtos;

public class PaymentRequestDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Receipt { get; set; } = string.Empty;
}
