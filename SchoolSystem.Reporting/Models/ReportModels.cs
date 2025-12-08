namespace SchoolSystem.Reporting.Models;

public class StudentPerformanceReport
{
    public int StudentId { get; set; }
    public double AverageScore { get; set; }
    public Dictionary<string, double> SubjectScores { get; set; } = new();
    public int TotalGrades { get; set; }
}

public class ClassPerformanceReport
{
    public int ClassId { get; set; }
    public double ClassAverage { get; set; }
    public List<StudentPerformanceReport> TopStudents { get; set; } = new();
}

public class AttendanceReport
{
    public int StudentId { get; set; }
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public double AttendancePercentage => TotalDays == 0 ? 0 : (double)PresentDays / TotalDays * 100;
}
