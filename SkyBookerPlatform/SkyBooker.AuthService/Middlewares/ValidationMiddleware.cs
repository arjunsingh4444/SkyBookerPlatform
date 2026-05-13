using SkyBooker.AuthService.Dtos;

namespace SkyBooker.AuthService.Middlewares;

// This middleware handles validation errors in a clean format.
// (The actual validation formatting is done in Program.cs → ConfigureApiBehaviorOptions)

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
    }
}
