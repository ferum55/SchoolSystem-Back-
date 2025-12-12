using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SchoolSystem.Notification.Consumers;
using SchoolSystem.Notification.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Notification Service API", Version = "v1" });
});

// DB Context
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlite("Data Source=notification.db"));

builder.Services.AddHttpClient("academic", c =>
{
    c.BaseAddress = new Uri("http://localhost:5004"); // Academic service
});


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GradeCreatedConsumer>();
    x.AddConsumer<HomeworkCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

//// MassTransit
//builder.Services.AddMassTransit(x =>
//{
//    x.AddConsumer<GradeCreatedConsumer>();
//    x.UsingRabbitMq((context, cfg) =>
//    {
//        cfg.Host("localhost", "/", h =>
//        {
//            h.Username("guest");
//            h.Password("guest");
//        });
//        cfg.ConfigureEndpoints(context);
//    });
//});

// OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource("SchoolSystem.Notification")
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("SchoolSystem.Notification"))
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter();
    });

var app = builder.Build();

// Auto-migrate - RESET DATABASE ON STARTUP (for testing)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

    //db.Database.EnsureDeleted(); // Delete existing database
    //db.Database.EnsureCreated(); // Create fresh database
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
