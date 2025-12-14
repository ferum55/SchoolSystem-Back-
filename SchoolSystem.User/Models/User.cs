namespace SchoolSystem.User.Models;

public class User
{
    public int Id { get; set; }
    public string PasswordHash { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
}

public enum UserRole
{
    Student,
    Teacher,
    Admin
}
