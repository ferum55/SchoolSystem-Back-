namespace SchoolSystem.Academic.Models;

public class Class
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "10-A"
    public int Year { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "Mathematics"
    public string Description { get; set; } = string.Empty;
}

public class ClassStudent
{
    public int ClassId { get; set; }
    public int StudentId { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}

public class TeacherSubject
{
    public int TeacherId { get; set; }
    public int SubjectId { get; set; }
    public int ClassId { get; set; }
}
