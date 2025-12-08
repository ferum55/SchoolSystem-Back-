using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Notification.Data;
using SchoolSystem.Notification.Models;
using SchoolSystem.Shared;

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

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserNotifications(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return Ok(notifications);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null) return NotFound();

        notification.IsRead = true;
        await _context.SaveChangesAsync();
        return NoContent();
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

    // Endpoint for manual notification creation (e.g. from other services via HTTP)
    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] NotificationEvent notificationEvent)
    {
        var notification = new Models.Notification
        {
            UserId = notificationEvent.StudentId,
            Type = NotificationType.Grade,
            Title = "New Grade Received",
            Message = $"You received a grade of {notificationEvent.Score} in {notificationEvent.Subject}",
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Notification created for user {notification.UserId}");
        
        return Ok(notification);
    }
}
