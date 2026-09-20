using DriveLog.Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriveLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly DriveLogDbContext _context;

    public LoginController(DriveLogDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.EmployeeId == request.EmployeeId);

        if (user == null)
        {
            return Unauthorized("Invalid employee ID or password.");
        }

        if (user.PasswordHash != request.Password)
        {
            return Unauthorized("Invalid employee ID or password.");
        }

        if (user.Status != DriveLog.Data.Enums.UserStatus.Active)
        {
            return Unauthorized("User account is not active.");
        }

        return Ok(new LoginResponse
        {
            UserId = user.Id,
            EmployeeId = user.EmployeeId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString()
        });
    }
}

public class LoginRequest
{
    public string EmployeeId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public int UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}