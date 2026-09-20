using DriveLog.Api.DTOs;
using DriveLog.Data.Data;
using DriveLog.Data.Models;
using DriveLog.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriveLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly DriveLogDbContext _context;
    private readonly DriveLogPdfService _pdfService;

    public ReportsController(
        DriveLogDbContext context,
        DriveLogPdfService pdfService)
    {
        _context = context;
        _pdfService = pdfService;
    }

    [HttpGet("driver/{driverId}")]
    public async Task<IActionResult> GetDriverReport(int driverId)
    {
        var driver = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == driverId);

        if (driver == null)
        {
            return NotFound("Driver not found.");
        }

        var driveLogs = await _context.DriveLogs
            .Where(d =>
                d.UserId == driverId &&
                d.EndTime != null)
            .OrderByDescending(d => d.StartTime)
            .Select(d => new DriveLogDto
            {
                Id = d.Id,
                UserId = d.UserId,
                VehicleId = d.VehicleId,
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                StartLocation = d.StartLocation,
                EndLocation = d.EndLocation,
                Purpose = d.purpose
            })
            .ToListAsync();

        return Ok(driveLogs);
    }

    [HttpGet("vehicle/{vehicleId}")]
    public async Task<IActionResult> GetVehicleReport(int vehicleId)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        var driveLogs = await _context.DriveLogs
            .Where(d =>
                d.VehicleId == vehicleId &&
                d.EndTime != null)
            .OrderByDescending(d => d.StartTime)
            .Select(d => new DriveLogDto
            {
                Id = d.Id,
                UserId = d.UserId,
                VehicleId = d.VehicleId,
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                StartLocation = d.StartLocation,
                EndLocation = d.EndLocation,
                Purpose = d.purpose
            })
            .ToListAsync();

        return Ok(driveLogs);
    }

    [HttpGet("driver/{driverId}/pdf")]
    public async Task<IActionResult> GetDriverPdf(int driverId)
    {
        var driver = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == driverId);

        if (driver == null)
        {
            return NotFound("Driver not found.");
        }

        var driveLogs = await _context.DriveLogs
            .Where(d =>
                d.UserId == driverId &&
                d.EndTime != null)
            .OrderByDescending(d => d.StartTime)
            .Select(d => new DriveLogDto
            {
                Id = d.Id,
                UserId = d.UserId,
                VehicleId = d.VehicleId,
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                StartLocation = d.StartLocation,
                EndLocation = d.EndLocation,
                Purpose = d.purpose
            })
            .ToListAsync();

        var pdf = _pdfService.GenerateReport(
            $"Driver Drive Report",
            $"{driver.FirstName} {driver.LastName}",
            driver.EmployeeId,
            "Multiple vehicles",
            driveLogs);

        return File(
            pdf,
            "application/pdf",
            $"Driver-{driver.EmployeeId}-DriveLog.pdf");
    }

    [HttpGet("vehicle/{vehicleId}/pdf")]
    public async Task<IActionResult> GetVehiclePdf(int vehicleId)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        var driveLogs = await _context.DriveLogs
            .Where(d =>
                d.VehicleId == vehicleId &&
                d.EndTime != null)
            .OrderByDescending(d => d.StartTime)
            .Select(d => new DriveLogDto
            {
                Id = d.Id,
                UserId = d.UserId,
                VehicleId = d.VehicleId,
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                StartLocation = d.StartLocation,
                EndLocation = d.EndLocation,
                Purpose = d.purpose
            })
            .ToListAsync();

        var pdf = _pdfService.GenerateReport(
            "Vehicle Drive Report",
            "Multiple drivers",
            "N/A",
            $"{vehicle.RegistrationNumber} - {vehicle.Make} {vehicle.Model}",
            driveLogs);

        return File(
            pdf,
            "application/pdf",
            $"Vehicle-{vehicle.RegistrationNumber}-DriveLog.pdf");
    }
}