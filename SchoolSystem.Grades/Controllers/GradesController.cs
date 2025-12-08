using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Grades.Data;
using SchoolSystem.Grades.Models;
using SchoolSystem.Shared;

namespace SchoolSystem.Grades.Controllers;

[ApiController]
[Route("grades")]
public class GradesController : ControllerBase
{
    private readonly GradesDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<GradesController> _logger;

    public GradesController(GradesDbContext context, IPublishEndpoint publishEndpoint, ILogger<GradesController> logger)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGrade([FromBody] Models.Grade grade)
    {
        grade.Date = DateTime.UtcNow;
        
        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Grade saved: {grade.Id}");

        // Publish event for Notification Service
        var notificationEvent = new NotificationEvent(grade.StudentId, grade.Id, "Subject", grade.Score, grade.Date);
        await _publishEndpoint.Publish(notificationEvent);

        return Ok(grade);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGrade(int id, Models.Grade grade)
    {
        if (id != grade.Id) return BadRequest();
        _context.Entry(grade).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGrade(int id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade == null) return NotFound();
        _context.Grades.Remove(grade);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
