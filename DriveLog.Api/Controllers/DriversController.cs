using DriveLog.Api.DTOs;
using DriveLog.Data.Data;
using DriveLog.Data.Enums;
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
        var drivers = await _context.Users
            .Where(u => u.Role == UserRole.Driver)
            .Select(u => new DriverDto
            {
                Id = u.Id,
                EmployeeId = u.EmployeeId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Status = u.Status.ToString()
            })
            .ToListAsync();

        return Ok(drivers);
    }

    [HttpGet("{id}/eligibility")]
    public async Task<IActionResult> GetEligibility(int id)
    {
        var driver = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (driver == null)
        {
            return NotFound("Driver not found.");
        }

        if (driver.Role != UserRole.Driver)
        {
            return BadRequest("User is not a driver.");
        }

        var documents = await _context.DriveDocuments
            .Where(d => d.UserId == driver.Id && d.IsCurrent)
            .ToListAsync();

        var hasValidDriversLicense = documents.Any(d =>
            d.DocumentType == DocumentType.License &&
            d.DocumentStatus == DocumentStatus.Approved &&
            d.ExpiryDate >= DateTime.UtcNow);

        var hasValidPDP = documents.Any(d =>
            d.DocumentType == DocumentType.PrDP &&
            d.DocumentStatus == DocumentStatus.Approved &&
            d.ExpiryDate >= DateTime.UtcNow);

        var isEligible =
            driver.Status == UserStatus.Active &&
            hasValidDriversLicense &&
            hasValidPDP;

        return Ok(new DriverEligibilityDto
        {
            UserId = driver.Id,
            IsEligible = isEligible,
            HasValidDriversLicense = hasValidDriversLicense,
            HasValidPDP = hasValidPDP,
            Message = isEligible
                ? "Driver is eligible to drive."
                : "Driver is not eligible to drive."
        });
    }
}