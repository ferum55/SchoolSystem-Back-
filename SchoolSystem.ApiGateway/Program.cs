using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Custom middleware to handle root and health endpoints BEFORE YARP
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            message = "School System API Gateway (YARP)",
            version = "2.0",
            status = "Running",
            technology = "YARP (Yet Another Reverse Proxy)",
            availableRoutes = new[]
            {
                new { path = "/grades/*", description = "Grades Service (Port 5001)", methods = new[] { "GET", "POST" } },
                new { path = "/notify/*", description = "Notification Service (Port 5002)", methods = new[] { "POST" } },
                new { path = "/users/*", description = "User Service (Port 5003)", methods = new[] { "GET", "POST" } },
                new { path = "/academic/*", description = "Academic Service (Port 5004)", methods = new[] { "GET", "POST" } },
                new { path = "/reports/*", description = "Reporting Service (Port 5005)", methods = new[] { "GET" } }
            },
            endpoints = new
            {
                health = "/health",
                individualSwagger = new
                {
                    grades = "http://localhost:5001/swagger",
                    users = "http://localhost:5003/swagger",
                    academic = "http://localhost:5004/swagger",
                    notification = "http://localhost:5002/swagger",
                    reporting = "http://localhost:5005/swagger"
                }
            }
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        return;
    }
    
    if (context.Request.Path == "/health")
    {
        context.Response.ContentType = "application/json";
        var response = new { status = "Healthy", timestamp = DateTime.UtcNow, gateway = "YARP" };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        return;
    }
    
    await next();
});

// Map YARP reverse proxy
app.MapReverseProxy();

app.Run();
