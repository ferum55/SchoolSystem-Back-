using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Reporting.Models;

namespace SchoolSystem.Reporting.Controllers;

[ApiController]
[Route("reports")]
public class ReportsController : ControllerBase
{
    public ReportsController()
    {
    }

    [HttpGet("student/{studentId}/performance")]
    public IActionResult GetStudentPerformance(int studentId)
    {
        // Mock data for demonstration
        var report = new StudentPerformanceReport
        {
            StudentId = studentId,
            AverageScore = 85.5,
            SubjectScores = new Dictionary<string, double>
            {
                { "Mathematics", 90 },
                { "History", 81 }
            },
            TotalGrades = 15
        };
        return Ok(report);
    }

    [HttpGet("class/{classId}/performance")]
    public IActionResult GetClassPerformance(int classId)
    {
        // Mock data
        var report = new ClassPerformanceReport
        {
            ClassId = classId,
            ClassAverage = 82.3,
            TopStudents = new List<StudentPerformanceReport>
            {
                new StudentPerformanceReport { StudentId = 1, AverageScore = 95 },
                new StudentPerformanceReport { StudentId = 2, AverageScore = 92 }
            }
        };
        return Ok(report);
    }

    [HttpGet("student/{studentId}/attendance")]
    public IActionResult GetStudentAttendance(int studentId)
    {
        // Mock data
        var report = new AttendanceReport
        {
            StudentId = studentId,
            TotalDays = 100,
            PresentDays = 95,
            AbsentDays = 5
        };
        return Ok(report);
    }
}
