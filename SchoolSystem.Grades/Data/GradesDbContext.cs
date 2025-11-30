using Microsoft.EntityFrameworkCore;

namespace SchoolSystem.Grades.Data;

public class Grade
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string StudentId { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public double Score { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class GradesDbContext : DbContext
{
    public GradesDbContext(DbContextOptions<GradesDbContext> options) : base(options) { }
    public DbSet<Grade> Grades { get; set; }
}
