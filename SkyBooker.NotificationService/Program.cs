using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SkyBooker.NotificationService.Data;

var builder = WebApplication.CreateBuilder(args);

// Database Setup
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SkyBooker-NotificationService", Version = "v1" });
});

var app = builder.Build();

// Database Migrations
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<NotificationDbContext>().Database.Migrate();
}

// Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkyBooker-NotificationService v1"));

app.UseCors("AllowAll");
app.MapControllers();

app.Run();
