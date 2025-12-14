using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Grades.Data;
using SchoolSystem.Grades.Models;
using SchoolSystem.Shared;
using System.Diagnostics;

namespace SchoolSystem.Grades.Controllers;

[ApiController]
[Route("grades")]
public class GradesController : ControllerBase
{
    private readonly GradesDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<GradesController> _logger;
    private static readonly ActivitySource ActivitySource =
    new("SchoolSystem.Grades");


public GradesController(GradesDbContext context, IPublishEndpoint publishEndpoint, ILogger<GradesController> logger)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGrade([FromBody] Grade grade)
    {
        using var activity = ActivitySource.StartActivity("CreateGrade");
        grade.Date = DateTime.UtcNow;

        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Grade saved: {grade.Id}");

        await _publishEndpoint.Publish(new GradeCreatedEvent
        {
            StudentId = grade.StudentId,
            ClassId = grade.ClassId,
            SubjectId = grade.SubjectId,
            GradeId = grade.Id,
            Score = grade.Score,
            GradeType = grade.Type.ToString()
        });



        return Ok(grade);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGrade(int id, Grade grade)
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

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetStudentGrades(int studentId)
    {
        var grades = await _context.Grades
            .Where(g => g.StudentId == studentId)
            .OrderByDescending(g => g.Date)
            .ToListAsync();

        return Ok(grades);
    }

    [HttpGet("student/{studentId}/subject/{subjectId}")]
    public async Task<IActionResult> GetStudentSubjectGrades(int studentId, int subjectId)
    {
        var grades = await _context.Grades
            .Where(g => g.StudentId == studentId && g.SubjectId == subjectId)
            .OrderByDescending(g => g.Date)
            .ToListAsync();

        return Ok(grades);
    }

    [HttpGet("class/{classId}/subject/{subjectId}")]
    public async Task<IActionResult> GetClassSubjectGrades(int classId, int subjectId)
    {
        var grades = await _context.Grades
            .Where(g => g.ClassId == classId && g.SubjectId == subjectId)
            .OrderBy(g => g.StudentId)
            .ThenBy(g => g.Date)
            .ToListAsync();

        return Ok(grades);
    }

    [HttpGet]
    public async Task<IActionResult> GetGrades([FromQuery] int classId, [FromQuery] int subjectId)
    {
        var grades = await _context.Grades
            .Where(g => g.ClassId == classId && g.SubjectId == subjectId)
            .ToListAsync();

        return Ok(grades);
    }

}
