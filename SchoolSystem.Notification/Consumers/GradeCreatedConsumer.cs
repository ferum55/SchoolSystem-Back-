using MassTransit;
using SchoolSystem.Notification.Data;
using SchoolSystem.Notification.Models;
using SchoolSystem.Shared;

namespace SchoolSystem.Notification.Consumers;

public class GradeCreatedConsumer : IConsumer<NotificationEvent>
{
    private readonly NotificationDbContext _context;
    private readonly ILogger<GradeCreatedConsumer> _logger;

    public GradeCreatedConsumer(NotificationDbContext context, ILogger<GradeCreatedConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NotificationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation($"Received grade notification for student {message.StudentId}");

        var notification = new Models.Notification
        {
            UserId = message.StudentId,
            Type = NotificationType.Grade,
            Title = "New Grade Received",
            Message = $"You received a grade of {message.Score} in {message.Subject}",
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Notification saved for user {notification.UserId}");
    }
}
