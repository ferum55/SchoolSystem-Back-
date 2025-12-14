using Microsoft.EntityFrameworkCore;
using SchoolSystem.User.Models;

namespace SchoolSystem.User.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<Models.User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
