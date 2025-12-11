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
        var attendance = await _context.Attendance
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return Ok(attendance);
    }


    [HttpGet("class/{classId}")]
    public async Task<IActionResult> GetClassAttendance(int classId)
    {
        var attendance = await _context.Attendance
            .Where(a => a.ClassId == classId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return Ok(attendance);
    }


    [HttpGet("student/{studentId}/subject/{subjectId}")]
    public async Task<IActionResult> GetStudentSubjectAttendance(int studentId, int subjectId)
    {
        var attendance = await _context.Attendance
            .Where(a => a.StudentId == studentId && a.SubjectId == subjectId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return Ok(attendance);
    }


    [HttpGet("class/{classId}/subject/{subjectId}")]
    public async Task<IActionResult> GetClassSubjectAttendance(int classId, int subjectId)
    {
        var attendance = await _context.Attendance
            .Where(a => a.ClassId == classId && a.SubjectId == subjectId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return Ok(attendance);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAttendance(int id, [FromBody] Attendance attendance)
    {
        var existing = await _context.Attendance.FindAsync(id);

        if (existing == null)
            return NotFound();


        existing.StudentId = attendance.StudentId;
        existing.ClassId = attendance.ClassId;
        existing.SubjectId = attendance.SubjectId;
        existing.Status = attendance.Status;
        existing.Reason = attendance.Reason;
        existing.Date = attendance.Date;

        await _context.SaveChangesAsync();

        return Ok(existing);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAttendance(int id)
    {
        var att = await _context.Attendance.FindAsync(id);
        if (att == null) return NotFound();

        _context.Attendance.Remove(att);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
