
using SmartEmergency.Application.DependencyInjection;
using SmartEmergency.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register Application Services
builder.Services.AddApplication();

// Register Infrastructure Services
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
