using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Reporting Service API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    [HttpGet("student/{id}")]
    public IActionResult GetStudentReport(string id)
    {
        return Ok(new { StudentId = id, AverageScore = 85.5, Attendance = "95%" });
    }
}
