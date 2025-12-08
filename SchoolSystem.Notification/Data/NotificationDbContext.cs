using Microsoft.EntityFrameworkCore;
using SchoolSystem.Notification.Models;

namespace SchoolSystem.Notification.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Notification> Notifications { get; set; }
}
