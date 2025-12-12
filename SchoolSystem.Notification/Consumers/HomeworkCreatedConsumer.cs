using MassTransit;
using SchoolSystem.Notification.Data;
using SchoolSystem.Notification.Models;
using SchoolSystem.Shared;
using System.Net.Http.Json;

namespace SchoolSystem.Notification.Consumers;

public class HomeworkCreatedConsumer : IConsumer<HomeworkCreatedEvent>
{
    private readonly NotificationDbContext _context;
    private readonly HttpClient _http;

    public HomeworkCreatedConsumer(
        NotificationDbContext context,
        IHttpClientFactory httpFactory)
    {
        _context = context;
        _http = httpFactory.CreateClient("academic");

    }

    public async Task Consume(ConsumeContext<HomeworkCreatedEvent> ctx)
    {
        var e = ctx.Message;

        // 1) Отримуємо студентів класу з Academic service
        var students = await _http.GetFromJsonAsync<List<StudentDto>>(
    $"classes/{e.ClassId}/students"
);


        if (students == null || students.Count == 0)
            return;

        // 2) Створюємо notification кожному студенту
        foreach (var s in students)
        {
            _context.Notifications.Add(new SchoolSystem.Notification.Models.Notification
            {
                UserId = s.Id,
                Type = SchoolSystem.Notification.Models.NotificationType.Homework,
                Title = "New homework assigned",
                Message = $"{e.Title}. Due: {e.DueDate:dd.MM.yyyy}",
                ClassId = e.ClassId,
                SubjectId = e.SubjectId,
                EntityId = e.HomeworkId,
                IsRead = false
            });
        }

        await _context.SaveChangesAsync();
    }
}

// DTO локально (мінімальний)
public record StudentDto(int Id, string FirstName, string LastName);
