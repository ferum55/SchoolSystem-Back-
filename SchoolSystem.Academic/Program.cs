using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Academic Service API", Version = "v1" });
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
public class AcademicController : ControllerBase
{
    [HttpGet("classes/{id}")]
    public IActionResult GetClass(string id)
    {
        return Ok(new { Id = id, Name = "Math 101", TeacherId = "teacher1" });
    }
}
