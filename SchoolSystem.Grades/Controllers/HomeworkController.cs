using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Grades.Data;
using SchoolSystem.Grades.Models;

namespace SchoolSystem.Grades.Controllers;

[ApiController]
[Route("homework")]
public class HomeworkController : ControllerBase
{
    private readonly GradesDbContext _context;

    public HomeworkController(GradesDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateHomework([FromBody] Homework homework)
    {
        homework.CreatedAt = DateTime.UtcNow;
        _context.Homework.Add(homework);
        await _context.SaveChangesAsync();
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
}
