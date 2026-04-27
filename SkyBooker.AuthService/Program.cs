using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SkyBooker.AuthService.Data;
using SkyBooker.AuthService.Dtos;
using SkyBooker.AuthService.Interfaces;
using SkyBooker.AuthService.Middlewares;
using SkyBooker.AuthService.Repository;
using SkyBooker.AuthService.Services;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// 1. DATABASE SETUP (SQL Server)
// =============================================
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// =============================================
// 2. DEPENDENCY INJECTION (DI)
// Tell the app: "when someone asks for IUserRepository, give them UserRepository"
// =============================================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthServiceImpl>();
builder.Services.AddScoped<ITokenService, TokenService>();

// =============================================
// 3. JWT AUTHENTICATION SETUP
// =============================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
string secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // These rules tell the app how to validate incoming JWT tokens
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,              // Check who created the token
        ValidateAudience = true,            // Check who the token is for
        ValidateLifetime = true,            // Check if token is expired
        ValidateIssuerSigningKey = true,     // Check the signature
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    // Custom error messages when token is missing or invalid
    options.Events = new JwtBearerEvents
    {
        // When user sends no token or an invalid token → 401
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var response = ApiResponse.Fail("Unauthorized. Please provide a valid token.");
            await context.Response.WriteAsJsonAsync(response);
        },

        // When user doesn't have the right role → 403
        OnForbidden = async context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";

            var response = ApiResponse.Fail("Forbidden. You don't have permission to access this resource.");
            await context.Response.WriteAsJsonAsync(response);
        }
    };
});

builder.Services.AddAuthorization();

// =============================================
// 4. CONTROLLERS + VALIDATION ERROR FORMAT
// =============================================
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // When DTO validation fails (e.g. missing email), return our standard format
        options.InvalidModelStateResponseFactory = context =>
        {
            // Collect all validation error messages
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(err => err.ErrorMessage))
                .ToList();

            // Join all errors into one message and return as BadRequest
            string errorMessage = string.Join("; ", errors);
            var response = ApiResponse.Fail(errorMessage);
            return new BadRequestObjectResult(response);
        };
    });

// =============================================
// 5. CORS (Allow all origins for development)
// =============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// =============================================
// 6. SWAGGER SETUP (API testing UI)
// =============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // API info shown on Swagger page
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SkyBooker-AuthService",
        Version = "v1",
        Description = "Authentication & Authorization service for the SkyBooker airline booking platform"
    });

    // Add the "Authorize" button in Swagger so we can test protected APIs
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token (without 'Bearer ' prefix)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// =============================================
// BUILD THE APP
// =============================================
var app = builder.Build();

// Auto-create the database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
}

// =============================================
// MIDDLEWARE PIPELINE (order matters!)
// =============================================

// Swagger UI (for testing APIs in browser)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkyBooker-AuthService v1");
    options.RoutePrefix = "swagger";
});

// Global error handler (catches any unhandled exception)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Allow cross-origin requests
app.UseCors("AllowAll");

// Authentication → checks JWT token
app.UseAuthentication();

// Authorization → checks if user has permission
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Start the server!
app.Run();
