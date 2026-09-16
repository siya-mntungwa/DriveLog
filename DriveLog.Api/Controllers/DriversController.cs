using DriveLog.Data.Data;
using DriveLog.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriveLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly DriveLogDbContext _context;

    public DriversController(DriveLogDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDrivers()
    {
        var drivers = await _context.Users.Where(u => u.Role == DriveLog.Data.Enums.UserRole.Driver)
        .Select(u => new DriverDto
        {
            Id = u.Id,
            EmployeeId = u.EmployeeId,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Status = u.Status.ToString()
        }).ToListAsync();
        return Ok(drivers);
    }
}