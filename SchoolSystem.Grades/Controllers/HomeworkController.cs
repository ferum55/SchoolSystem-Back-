using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Grades.Data;
using SchoolSystem.Grades.Models;
using SchoolSystem.Shared;
using MassTransit;

namespace SchoolSystem.Grades.Controllers;

public record StudentDto(int Id, string FirstName, string LastName);


[ApiController]
[Route("homework")]
public class HomeworkController : ControllerBase
{
    private readonly GradesDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;


    public HomeworkController(GradesDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> CreateHomework([FromBody] Homework homework)
    {
        homework.CreatedAt = DateTime.UtcNow;
        _context.Homework.Add(homework);
        await _context.SaveChangesAsync();
        await _publishEndpoint.Publish(new HomeworkCreatedEvent
        {
            ClassId = homework.ClassId,
            SubjectId = homework.SubjectId,
            HomeworkId = homework.Id,
            Title = homework.Title,
            DueDate = homework.DueDate
        });

        return Ok(homework);
    }

    [HttpGet("class/{classId}")]
    public async Task<IActionResult> GetClassHomework(int classId)
    {
        var homework = await _context.Homework
            .Where(h => h.ClassId == classId)
            .OrderByDescending(h => h.DueDate)
            .ToListAsync();

        return Ok(homework);
    }

    [HttpGet("class/{classId}/subject/{subjectId}")]
    public async Task<IActionResult> GetClassSubjectHomework(int classId, int subjectId)
    {
        var homework = await _context.Homework
            .Where(h => h.ClassId == classId && h.SubjectId == subjectId)
            .OrderByDescending(h => h.DueDate)
            .ToListAsync();

        return Ok(homework);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHomework(int id, Homework homework)
    {
        if (id != homework.Id) return BadRequest();
        _context.Entry(homework).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHomework(int id)
    {
        var homework = await _context.Homework.FindAsync(id);
        if (homework == null) return NotFound();
        _context.Homework.Remove(homework);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("classic")]
    public async Task<IActionResult> CreateHomeworkClassic(
    [FromBody] Homework homework,
    [FromServices] IHttpClientFactory httpFactory)
    {
        homework.CreatedAt = DateTime.UtcNow;
        _context.Homework.Add(homework);
        await _context.SaveChangesAsync();

        var http = httpFactory.CreateClient();

        var students = await http.GetFromJsonAsync<List<StudentDto>>(
            $"http://localhost:5004/classes/{homework.ClassId}/students"
        );

        if (students == null)
            return StatusCode(500, "Failed to load students");
        foreach (var s in students)
        {
            await http.PostAsJsonAsync(
                "http://localhost:5006/notifications/classic",
                new CreateNotificationRequest
                {
                    UserId = s.Id,
                    Type = NotificationType.Homework,
                    Title = "New homework assigned",
                    Message = $"{homework.Title}. Due: {homework.DueDate:dd.MM.yyyy}",
                    ClassId = homework.ClassId,
                    SubjectId = homework.SubjectId,
                    EntityId = homework.Id
                }
            );
        }

        return Ok(homework);
    }

}
