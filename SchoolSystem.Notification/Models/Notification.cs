namespace SchoolSystem.Notification.Models;

public enum NotificationType
{
    Grade,
    Homework
}

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public NotificationType Type { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int EntityId { get; set; }

    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


