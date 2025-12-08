using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Grades.Data;
using SchoolSystem.Grades.Models;

namespace SchoolSystem.Grades.Controllers;

[ApiController]
[Route("attendance")]
public class AttendanceController : ControllerBase
{
    private readonly GradesDbContext _context;

    public AttendanceController(GradesDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> MarkAttendance([FromBody] Attendance attendance)
    {
        attendance.Date = DateTime.UtcNow;
        _context.Attendance.Add(attendance);
        await _context.SaveChangesAsync();
        return Ok(attendance);
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetStudentAttendance(int studentId)
    {
        var attendance = await _context.Attendance.Where(a => a.StudentId == studentId).ToListAsync();
        return Ok(attendance);
    }

    [HttpGet("class/{classId}")]
    public async Task<IActionResult> GetClassAttendance(int classId)
    {
        var attendance = await _context.Attendance.Where(a => a.ClassId == classId).ToListAsync();
        return Ok(attendance);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAttendance(int id, Attendance attendance)
    {
        if (id != attendance.Id) return BadRequest();
        _context.Entry(attendance).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
