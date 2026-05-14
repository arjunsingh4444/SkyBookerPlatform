using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using SkyBooker.NotificationService.Dtos;

namespace SkyBooker.NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IConfiguration configuration, ILogger<PaymentController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("create-order")]
    public IActionResult CreateOrder([FromBody] PaymentRequestDto request)
    {
        try
        {
            // Read from appsettings or use test keys (replace with real keys in production)
            string keyId = _configuration["Razorpay:KeyId"] ?? "rzp_test_Sp8rGffhZPzObU";
            string keySecret = _configuration["Razorpay:KeySecret"] ?? "FtTJHlqJX0TaoiiTO8rB7J9G";

            RazorpayClient client = new RazorpayClient(keyId, keySecret);

            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", (int)(request.Amount * 100)); // amount in the smallest currency unit (paise)
            options.Add("currency", request.Currency);
            options.Add("receipt", string.IsNullOrEmpty(request.Receipt) ? Guid.NewGuid().ToString() : request.Receipt);

            Order order = client.Order.Create(options);

            return Ok(new {
                orderId = order["id"].ToString(),
                amount = request.Amount,
                currency = request.Currency
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Razorpay order");
            return StatusCode(500, new { message = "Error creating order", details = ex.Message });
        }
    }

    [HttpPost("verify")]
    public IActionResult VerifyPayment([FromBody] PaymentVerificationDto request)
    {
        try
        {
            string keySecret = _configuration["Razorpay:KeySecret"] ?? "FtTJHlqJX0TaoiiTO8rB7J9G";

            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("razorpay_payment_id", request.PaymentId);
            attributes.Add("razorpay_order_id", request.OrderId);
            attributes.Add("razorpay_signature", request.Signature);
            attributes.Add("secret", keySecret);

            Utils.verifyPaymentSignature(attributes);

            return Ok(new { message = "Payment verified successfully" });
        }
        catch (Razorpay.Api.Errors.SignatureVerificationError ex)
        {
            _logger.LogWarning(ex, "Invalid payment signature");
            return BadRequest(new { message = "Payment verification failed: Invalid signature" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment");
            return StatusCode(500, new { message = "Error verifying payment", details = ex.Message });
        }
    }
}
