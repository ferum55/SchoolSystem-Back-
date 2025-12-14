using SchoolSystem.User.Models;

namespace SchoolSystem.User.DTOs;

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponse(
    string Token,
    int UserId,
    string Email,
    string FullName,
    string Role
);

