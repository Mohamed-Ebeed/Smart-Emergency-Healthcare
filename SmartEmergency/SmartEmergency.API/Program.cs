using SmartEmergency.API.Extensions;
using SmartEmergency.API.Hubs;
using SmartEmergency.API.Middleware;
using SmartEmergency.Application.DependencyInjection;
using SmartEmergency.Application.Notifications.Interfaces;
using SmartEmergency.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register Application Services
builder.Services.AddApplication();

// Register Infrastructure Services
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// SignalR (live updates). Replaces the do-nothing defaults from AddApplication
builder.Services.AddSignalR();
builder.Services.AddScoped<IRealtimeNotifier, SignalRRealtimeNotifier>();
builder.Services.AddScoped<INotificationSender, SignalRNotificationSender>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("WebClient", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// OpenAPI (with JWT button)
builder.Services.AddOpenApiWithJwt();

var app = builder.Build();

// Global Exception Handling
app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Smart Emergency API v1");
    });
}

// HTTP Pipeline
app.UseHttpsRedirection();

app.UseCors("WebClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<EmergencyHub>("/hubs/emergency");

app.Run();