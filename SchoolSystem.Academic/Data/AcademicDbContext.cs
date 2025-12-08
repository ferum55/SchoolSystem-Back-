using Microsoft.EntityFrameworkCore;
using SchoolSystem.Academic.Models;

namespace SchoolSystem.Academic.Data;

public class AcademicDbContext : DbContext
{
    public AcademicDbContext(DbContextOptions<AcademicDbContext> options) : base(options)
    {
    }

    public DbSet<Class> Classes { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<ClassStudent> ClassStudents { get; set; }
    public DbSet<TeacherSubject> TeacherSubjects { get; set; }
    public DbSet<UserRef> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClassStudent>()
            .HasKey(cs => new { cs.ClassId, cs.StudentId });

        modelBuilder.Entity<TeacherSubject>()
            .HasKey(ts => new { ts.TeacherId, ts.SubjectId, ts.ClassId });
    }
}
