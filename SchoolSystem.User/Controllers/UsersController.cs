using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolSystem.User.Data;
using SchoolSystem.User.DTOs;
using SchoolSystem.User.Models;

namespace SchoolSystem.User.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly UserDbContext _context;
    private readonly IConfiguration _configuration;

    public UsersController(UserDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already exists");

        var user = new SchoolSystem.User.Models.User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwt(user);

        return Ok(new AuthResponse(
    token,
    user.Id,
    user.Email,
    $"{user.FirstName} {user.LastName}".Trim(),
    user.Role.ToString()
));

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            return Unauthorized("Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password");

        var token = GenerateJwt(user);

        return Ok(new AuthResponse(
    token,
    user.Id,
    user.Email,
    $"{user.FirstName} {user.LastName}".Trim(),
    user.Role.ToString()
));

    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Role = user.Role.ToString(),
            user.CreatedAt,
            user.LastLogin
        });

    }

    private string GenerateJwt(SchoolSystem.User.Models.User user)
    {
        var jwtSection = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSection["SecretKey"] ?? "super_secret_key_for_school_system_12345";
        var issuer = jwtSection["Issuer"] ?? "SchoolSystem";
        var audience = jwtSection["Audience"] ?? "SchoolSystem";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
        new Claim("email", user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
