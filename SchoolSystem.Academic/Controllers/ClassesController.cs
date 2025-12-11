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

    [HttpGet("teacher/{teacherId}")]
    public async Task<IActionResult> GetTeacherClasses(int teacherId)
    {
        var classes = await _context.TeacherSubjects
            .Where(ts => ts.TeacherId == teacherId)
            .Select(ts => ts.ClassId)
            .Distinct()
            .Join(_context.Classes,
                classId => classId,
                c => c.Id,
                (classId, c) => new ClassDto(c.Id, c.Name, c.Year))
            .ToListAsync();

        return Ok(classes);
    }


    [HttpPost]
    public async Task<IActionResult> CreateClass(Class classObj)
    {
        classObj.CreatedAt = DateTime.UtcNow;
        _context.Classes.Add(classObj);
        await _context.SaveChangesAsync();
        return Ok(new ClassDto(classObj.Id, classObj.Name, classObj.Year));
    }

    [HttpGet("{classId}/info")]
    public async Task<IActionResult> GetClassInfo(int classId)
    {
        var cls = await _context.Classes.FindAsync(classId);
        if (cls == null) return NotFound("Class not found");

        var subjects = await _context.TeacherSubjects
            .Where(ts => ts.ClassId == classId)
            .Join(_context.Subjects,
                ts => ts.SubjectId,
                s => s.Id,
                (ts, s) => new SubjectDto(s.Id, s.Name, s.Description))
            .ToListAsync();

        return Ok(new
        {
            id = cls.Id,
            name = cls.Name,
            year = cls.Year,
            subjects = subjects
        });
    }

    [HttpGet("by-student/{studentId}")]
    public async Task<IActionResult> GetClassForStudent(int studentId)
    {
        // «находимо запис зв'€зку "клас Ц студент"
        var link = await _context.ClassStudents
            .FirstOrDefaultAsync(cs => cs.StudentId == studentId);

        if (link == null)
            return NotFound($"Student {studentId} is not assigned to any class");

        var classObj = await _context.Classes.FindAsync(link.ClassId);
        if (classObj == null)
            return NotFound($"Class {link.ClassId} not found");

        return Ok(new ClassDto(classObj.Id, classObj.Name, classObj.Year));
    }


}
