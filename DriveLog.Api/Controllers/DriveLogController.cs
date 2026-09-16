using DriveLog.Data.Data;
using DriveLog.Api.DTOs;
using DriveLog.Data.Enums;
using DriveLog.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriveLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriveLogsController : ControllerBase
{
    private readonly DriveLogDbContext _context;

    public DriveLogsController(DriveLogDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDriveLogs()
    {
        var driveLogs = await _context.DriveLogs.Select(d => new DriveLogDto
            {
                Id = d.Id,
                UserId = d.UserId,
                VehicleId = d.VehicleId,
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                StartLocation = d.StartLocation,
                EndLocation = d.EndLocation,
                Purpose = d.purpose
            }).ToListAsync();

        return Ok(driveLogs);
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartDrive(StartDriveDto request)
    {
        var driver = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (driver == null)
        {
            return NotFound("Driver not found.");
        }

        if (driver.Role != UserRole.Driver)
        {
            return BadRequest("User is not a driver.");
        }

        if (driver.Status != UserStatus.Active)
        {
            return BadRequest("Driver is not active.");
        }

        var documents = await _context.DriveDocuments.Where(d => d.UserId == driver.Id && d.IsCurrent).ToListAsync();

        var hasValidDriversLicense = documents.Any(d =>
            d.DocumentType == DocumentType.License &&
            d.DocumentStatus == DocumentStatus.Approved &&
            d.ExpiryDate >= DateTime.UtcNow);

        var hasValidPDP = documents.Any(d =>
            d.DocumentType == DocumentType.PrDP &&
            d.DocumentStatus == DocumentStatus.Approved &&
            d.ExpiryDate >= DateTime.UtcNow);

        if (!hasValidDriversLicense || !hasValidPDP)
        {
            return BadRequest("Driver is not eligible to drive.");
        }

        var existingDriverDrive = await _context.DriveLogs.AnyAsync(d => d.UserId == driver.Id && d.EndTime == null);

        if (existingDriverDrive)
        {
            return BadRequest("Driver already has an active drive.");
        }

        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == request.VehicleId);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        if (vehicle.Status != VehicleStatus.Available)
        {
            return BadRequest("Vehicle is not available.");
        }

        var existingVehicleDrive = await _context.DriveLogs.AnyAsync(d => d.VehicleId == vehicle.Id && d.EndTime == null);

        if (existingVehicleDrive)
        {
            return BadRequest("Vehicle already has an active drive.");
        }

        var driveLog = new DriveLogEntry
        {
            UserId = driver.Id,
            VehicleId = vehicle.Id,
            StartTime = DateTime.UtcNow,
            StartLocation = request.StartLocation,
            purpose = request.Purpose
        };

        vehicle.Status = VehicleStatus.InUse;

        _context.DriveLogs.Add(driveLog);

        await _context.SaveChangesAsync();

        var driveLogDto = new DriveLogDto
        {
            Id = driveLog.Id,
            UserId = driveLog.UserId,
            VehicleId = driveLog.VehicleId,
            StartTime = driveLog.StartTime,
            EndTime = driveLog.EndTime,
            StartLocation = driveLog.StartLocation,
            EndLocation = driveLog.EndLocation,
            Purpose = driveLog.purpose
        };

        return Ok(driveLogDto);
    }

    [HttpPost("end/{id}")]
    public async Task<IActionResult> EndDrive(int id, EndDriveDto request)
    {
        var driveLog = await _context.DriveLogs.FirstOrDefaultAsync(d => d.Id == id);

        if (driveLog == null)
        {
            return NotFound("Drive not found.");
        }

        if (driveLog.EndTime != null)
        {
            return BadRequest("Drive has already ended.");
        }

        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == driveLog.VehicleId);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        driveLog.EndTime = DateTime.UtcNow;
        driveLog.EndLocation = request.EndLocation;

        vehicle.Status = VehicleStatus.Available;

        await _context.SaveChangesAsync();

        var driveLogDto = new DriveLogDto
        {
            Id = driveLog.Id,
            UserId = driveLog.UserId,
            VehicleId = driveLog.VehicleId,
            StartTime = driveLog.StartTime,
            EndTime = driveLog.EndTime,
            StartLocation = driveLog.StartLocation,
            EndLocation = driveLog.EndLocation,
            Purpose = driveLog.purpose
        };

        return Ok(driveLogDto);
    }
}