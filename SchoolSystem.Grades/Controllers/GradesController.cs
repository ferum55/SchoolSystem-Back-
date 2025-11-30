using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Grades.Data;
using SchoolSystem.Shared;

namespace SchoolSystem.Grades.Controllers;

[ApiController]
[Route("[controller]")]
public class GradesController : ControllerBase
{
    private readonly GradesDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GradesController> _logger;

    public GradesController(GradesDbContext context, IPublishEndpoint publishEndpoint, IHttpClientFactory httpClientFactory, ILogger<GradesController> logger)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeRequest request, [FromQuery] string mode = "classic")
    {
        var grade = new Grade
        {
            StudentId = request.StudentId,
            Subject = request.Subject,
            Score = request.Score
        };

        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Grade saved: {grade.Id} (Mode: {mode})");

        var notificationEvent = new NotificationEvent(grade.StudentId, grade.Id, grade.Subject, grade.Score, grade.Timestamp);

        if (mode == "classic")
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                // Assuming Notification Service runs on port 5002
                await client.PostAsJsonAsync("http://localhost:5002/notify", notificationEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send synchronous notification");
                return StatusCode(500, "Failed to send notification");
            }
        }
        else if (mode == "event")
        {
            await _publishEndpoint.Publish(notificationEvent);
        }

        return Ok(grade);
    }
}

public record CreateGradeRequest(string StudentId, string Subject, double Score);
