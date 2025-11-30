namespace SchoolSystem.Shared;

public record GradeDTO(string Id, string StudentId, string Subject, double Score, DateTime Timestamp);

public record NotificationEvent(string StudentId, string GradeId, string Subject, double Score, DateTime Timestamp);
