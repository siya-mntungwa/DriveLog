using DriveLog.Api.DTOs;
using DriveLog.Data.Data;
using DriveLog.Data.Enums;
using DriveLog.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriveLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly DriveLogDbContext _context;

    public UsersController(DriveLogDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.EmployeeId) ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("All user fields are required.");
        }

        var employeeExists = await _context.Users
            .AnyAsync(u => u.EmployeeId == request.EmployeeId);

        if (employeeExists)
        {
            return BadRequest("Employee ID already exists.");
        }

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (emailExists)
        {
            return BadRequest("Email already exists.");
        }

        var user = new User
        {
            EmployeeId = request.EmployeeId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = request.Password,
            Role = request.Role,
            Status = UserStatus.Active
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            user.Id,
            user.EmployeeId,
            user.FirstName,
            user.LastName,
            user.Email,
            Role = user.Role.ToString(),
            Status = user.Status.ToString()
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                EmployeeId = u.EmployeeId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role.ToString(),
                Status = u.Status.ToString()
            })
            .ToListAsync();

        return Ok(users);
    }
}