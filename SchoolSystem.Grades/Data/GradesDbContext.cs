using Microsoft.EntityFrameworkCore;
using SchoolSystem.Grades.Models;

namespace SchoolSystem.Grades.Data;



public class GradesDbContext : DbContext
{
    public GradesDbContext(DbContextOptions<GradesDbContext> options) : base(options) { }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Attendance> Attendance { get; set; }
    public DbSet<Homework> Homework { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Additional configuration if needed
    }
}
