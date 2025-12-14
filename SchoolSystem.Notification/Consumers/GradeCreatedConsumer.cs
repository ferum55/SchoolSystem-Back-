using MassTransit;
using SchoolSystem.Notification.Data;
using SchoolSystem.Notification.Models;
using SchoolSystem.Shared;
using System.Diagnostics;
using DbNotificationType = SchoolSystem.Notification.Models.NotificationType;
using SharedNotificationType = SchoolSystem.Shared.NotificationType;
using System.Diagnostics;


namespace SchoolSystem.Notification.Consumers;

public class GradeCreatedConsumer : IConsumer<GradeCreatedEvent>
{
    private readonly NotificationDbContext _context;
    private static readonly ActivitySource ActivitySource =
    new("SchoolSystem.Notification");

    public GradeCreatedConsumer(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<GradeCreatedEvent> ctx)
    {
        using var activity = ActivitySource.StartActivity("HandleGradeCreated");
        var e = ctx.Message;

        var notification = new SchoolSystem.Notification.Models.Notification
        {
            UserId = e.StudentId,
            Type = SchoolSystem.Notification.Models.NotificationType.Grade,
            Title = "New grade received",
            Message = $"Grade: {e.Score} ({e.GradeType})",
            ClassId = e.ClassId,
            SubjectId = e.SubjectId,
            EntityId = e.GradeId,
            IsRead = false
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}

