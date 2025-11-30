using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Shared;

namespace SchoolSystem.Notification.Controllers;

[ApiController]
[Route("[controller]")]
public class NotifyController : ControllerBase
{
    private readonly ILogger<NotifyController> _logger;

    public NotifyController(ILogger<NotifyController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Notify([FromBody] NotificationEvent notification)
    {
        _logger.LogInformation($"[Classic] Received notification for Student {notification.StudentId}: {notification.Subject} - {notification.Score}");
        
        // Simulate processing
        await Task.Delay(100);

        return Ok("Notification received");
    }
}
