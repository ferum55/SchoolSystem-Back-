using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Notification.Data;
using SchoolSystem.Notification.Models;
using SchoolSystem.Shared;


using SharedNotificationType = SchoolSystem.Shared.NotificationType;
using DbNotificationType = SchoolSystem.Notification.Models.NotificationType;


namespace SchoolSystem.Notification.Controllers;

[ApiController]
[Route("notifications")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationDbContext _context;
    private readonly ILogger<NotificationsController> _logger;


    public NotificationsController(NotificationDbContext context, ILogger<NotificationsController> logger)
    {
        _context = context;
        _logger = logger;
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null) return NotFound();

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserNotifications(int userId, int page = 1, int pageSize = 10)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }


    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var n = await _context.Notifications.FindAsync(id);
        if (n == null) return NotFound();

        n.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            n.Type,
            n.ClassId,
            n.SubjectId,
            n.EntityId
        });
    }

    [HttpPost("classic")]
    public async Task<IActionResult> CreateClassic([FromBody] CreateNotificationRequest req)
    {
        var notification = new SchoolSystem.Notification.Models.Notification
        {
            UserId = req.UserId,
            Type = req.Type == SchoolSystem.Shared.NotificationType.Homework
                ? Models.NotificationType.Homework
                : Models.NotificationType.Grade,

            Title = req.Title,
            Message = req.Message,

            ClassId = req.ClassId,
            SubjectId = req.SubjectId,
            EntityId = req.EntityId,

            IsRead = false
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return Ok();
    }



}
