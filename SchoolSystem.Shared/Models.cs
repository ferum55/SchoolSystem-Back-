
namespace SchoolSystem.Shared;




public record GradeDTO(int Id, int StudentId, string Subject, double Score, DateTime Timestamp);


public enum NotificationType
{
    Grade,
    Homework
}

public class HomeworkCreatedEvent
{
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int HomeworkId { get; set; }

    public string Title { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
}

public class GradeCreatedEvent
{
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int GradeId { get; set; }

    public double Score { get; set; }
    public string GradeType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CreateNotificationRequest
{
    public int UserId { get; set; }
    public NotificationType Type { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int EntityId { get; set; }
}
