using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Academic.Data;
using SchoolSystem.Academic.Models;
using SchoolSystem.Academic.DTOs;

namespace SchoolSystem.Academic.Controllers;

[ApiController]
[Route("subjects")]
public class SubjectsController : ControllerBase
{
    private readonly AcademicDbContext _context;

    public SubjectsController(AcademicDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetSubjects()
    {
        var subjects = await _context.Subjects
            .Select(s => new SubjectDto(s.Id, s.Name, s.Description))
            .ToListAsync();

        return Ok(subjects);
    }

    [HttpGet("teacher/{teacherId}")]
    public async Task<IActionResult> GetTeacherSubjects(int teacherId)
    {
        var result = await _context.TeacherSubjects
            .Where(ts => ts.TeacherId == teacherId)
            .Join(_context.Subjects,
                ts => ts.SubjectId,
                s => s.Id,
                (ts, s) => new TeacherSubjectDto(
                    ts.TeacherId,
                    ts.SubjectId,
                    ts.ClassId,
                    s.Name,
                    _context.Classes.First(c => c.Id == ts.ClassId).Name
                ))
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("{id}/info")]
    public async Task<IActionResult> GetSubjectInfo(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null) return NotFound("Subject not found");

        return Ok(new
        {
            id = subject.Id,
            name = subject.Name,
            description = subject.Description
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubject(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null) return NotFound();

        return Ok(new SubjectDto(subject.Id, subject.Name, subject.Description));
    }


}
