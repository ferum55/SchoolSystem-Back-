using MassTransit;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Grades.Data;
using SchoolSystem.Shared;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter; // Для ZipkinExportProtocol
using OpenTelemetry.Instrumentation.MassTransit; // Для AddMassTransitInstrumentation
using OpenTelemetry.Instrumentation.Http;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

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
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenTelemetry()
    .WithTracing(b =>
    {
        b
         .SetResourceBuilder(
             ResourceBuilder.CreateDefault()
                 .AddService("schoolsystem.grades")
         )
         .SetSampler(new AlwaysOnSampler())
         .AddSource("SchoolSystem.Grades")
         .AddAspNetCoreInstrumentation()
         .AddMassTransitInstrumentation()
         .AddZipkinExporter(o =>
         {
             o.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
         });
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
    //db.Database.EnsureDeleted(); // Delete existing database
    //db.Database.EnsureCreated(); // Create fresh database
}

app.UseAuthorization();

app.MapControllers();

app.Run();