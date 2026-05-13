using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SkyBooker.FlightService.Data;
using SkyBooker.FlightService.Dtos;
using SkyBooker.FlightService.Interfaces;
using SkyBooker.FlightService.Middlewares;
using SkyBooker.FlightService.Repository;
using SkyBooker.FlightService.Services;

var builder = WebApplication.CreateBuilder(args);

// Database Setup
builder.Services.AddDbContext<FlightDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IFlightService, FlightServiceImpl>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
        
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ApiResponse.Fail("Unauthorized. Please provide a valid token."));
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ApiResponse.Fail("Forbidden. You don't have permission."));
            }
        };
    });

builder.Services.AddAuthorization();

// Controllers and Validation
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values.SelectMany(e => e.Errors.Select(err => err.ErrorMessage));
        return new BadRequestObjectResult(ApiResponse.Fail(string.Join("; ", errors)));
    };
});

// CORS
builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SkyBooker-FlightService", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "Bearer", BearerFormat = "JWT", In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
});

var app = builder.Build();

// Database Migrations
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<FlightDbContext>().Database.Migrate();
}

// Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkyBooker-FlightService v1"));

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
