namespace SchoolSystem.Grades.Models;

public enum GradeType
{
    Current,
    Thematic,
    Semester,
    Yearly
}

public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    Excused
}

public class Grade
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int ClassId { get; set; }
    public int TeacherId { get; set; }
    public double Score { get; set; }
    public GradeType Type { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
}

public class Attendance
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public AttendanceStatus Status { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class Homework
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int ClassId { get; set; }
    public int TeacherId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
