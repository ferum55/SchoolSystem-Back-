using MassTransit;
using SchoolSystem.Shared;

namespace SchoolSystem.Notification.Consumers;

public class GradeCreatedConsumer : IConsumer<NotificationEvent>
{
    private readonly ILogger<GradeCreatedConsumer> _logger;

    public GradeCreatedConsumer(ILogger<GradeCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NotificationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation($"[Event] Received notification for Student {message.StudentId}: {message.Subject} - {message.Score}");
        
        // Simulate processing
        await Task.Delay(100);
    }
}
