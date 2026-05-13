using System.Net;
using System.Text.Json;
using SkyBooker.AuthService.Dtos;

namespace SkyBooker.AuthService.Middlewares;

// This middleware catches any unhandled errors in the app.
// Instead of showing ugly error pages, it returns a clean JSON response.
// Example: { "success": false, "message": "Internal Server Error: ..." }

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Try to process the request normally
            await _next(context);
        }
        catch (Exception ex)
        {
            // If any error happens, catch it here
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

            // Send a clean error response to the client
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;  // 500 = Internal Server Error

            var response = ApiResponse.Fail("Internal Server Error: " + ex.Message);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(json);
        }
    }
}
