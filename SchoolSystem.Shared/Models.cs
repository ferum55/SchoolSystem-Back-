namespace SchoolSystem.Shared;

public record GradeDTO(int Id, int StudentId, string Subject, double Score, DateTime Timestamp);

public record NotificationEvent(int StudentId, int GradeId, string Subject, double Score, DateTime Timestamp);
