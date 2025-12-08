using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Academic.Data;
using SchoolSystem.Academic.Models;
using SchoolSystem.Academic.DTOs;

namespace SchoolSystem.Academic.Controllers;

[ApiController]
[Route("classes")]
public class ClassesController : ControllerBase
{
    private readonly AcademicDbContext _context;

    public ClassesController(AcademicDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetClasses()
    {
        var classes = await _context.Classes
            .Select(c => new ClassDto(c.Id, c.Name, c.Year))
            .ToListAsync();

        return Ok(classes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClass(int id)
    {
        var classObj = await _context.Classes.FindAsync(id);
        if (classObj == null) return NotFound();

        return Ok(new ClassDto(classObj.Id, classObj.Name, classObj.Year));
    }

    [HttpGet("{id}/students")]
    public async Task<IActionResult> GetStudentsInClass(int id)
    {
        var students = await _context.ClassStudents
            .Where(cs => cs.ClassId == id)
            .Join(_context.Users,
                cs => cs.StudentId,
                u => u.Id,
                (cs, u) => new StudentDto(u.Id, u.FirstName, u.LastName))
            .ToListAsync();

        return Ok(students);
    }

    [HttpGet("{id}/subjects")]
    public async Task<IActionResult> GetSubjectsInClass(int id)
    {
        var subjects = await _context.TeacherSubjects
            .Where(ts => ts.ClassId == id)
            .Join(_context.Subjects,
                ts => ts.SubjectId,
                s => s.Id,
                (ts, s) => new SubjectDto(s.Id, s.Name, s.Description))
            .Distinct()
            .ToListAsync();

        return Ok(subjects);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClass(Class classObj)
    {
        classObj.CreatedAt = DateTime.UtcNow;
        _context.Classes.Add(classObj);
        await _context.SaveChangesAsync();
        return Ok(new ClassDto(classObj.Id, classObj.Name, classObj.Year));
    }
}
