using MassTransit;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Grades.Data;
using SchoolSystem.Shared;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// DB Context
builder.Services.AddDbContext<GradesDbContext>(options =>
    options.UseSqlite("Data Source=grades.db"));

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

// OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource("SchoolSystem.Grades")
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("SchoolSystem.Grades"))
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Auto-migrate - RESET DATABASE ON STARTUP (for testing)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GradesDbContext>();
    db.Database.EnsureDeleted(); // Delete existing database
    db.Database.EnsureCreated(); // Create fresh database
}

app.UseAuthorization();

app.MapControllers();

app.Run();
