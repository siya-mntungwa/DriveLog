using DriveLog.Api.DTOs;
using DriveLog.Data.Data;
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

    // =========================
    // DRIVER REPORT
    // =========================

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

                Customer = d.Customer,
                DeliveryNoteId = d.DeliveryNoteId,

                StartKm = d.StartKm,
                EndKm = d.EndKm,

                Purpose = d.Purpose
            })
            .ToListAsync();

        return Ok(driveLogs);
    }

    // =========================
    // VEHICLE REPORT
    // =========================

    [HttpGet("vehicle/{vehicleId}")]
    public async Task<IActionResult> GetVehicleReport(int vehicleId)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        var driveLogs = await (
            from drive in _context.DriveLogs
            join user in _context.Users
                on drive.UserId equals user.Id
            where drive.VehicleId == vehicleId
                && drive.EndTime != null
            orderby drive.StartTime descending
            select new DriveLogDto
            {
                Id = drive.Id,
                UserId = drive.UserId,
                VehicleId = drive.VehicleId,

                StartTime = drive.StartTime,
                EndTime = drive.EndTime,

                StartLocation = drive.StartLocation,
                EndLocation = drive.EndLocation,

                Customer = drive.Customer,
                DeliveryNoteId = drive.DeliveryNoteId,

                StartKm = drive.StartKm,
                EndKm = drive.EndKm,

                Purpose = drive.Purpose,

                DriverName = user.FirstName + " " + user.LastName
            }
        ).ToListAsync();

        return Ok(driveLogs);
    }

    // =========================
    // DRIVER PDF
    // =========================

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

                Customer = d.Customer,
                DeliveryNoteId = d.DeliveryNoteId,

                StartKm = d.StartKm,
                EndKm = d.EndKm,

                Purpose = d.Purpose
            })
            .ToListAsync();

        var pdf = _pdfService.GenerateReport(
            "Driver Drive Report",
            $"{driver.FirstName} {driver.LastName}",
            "Multiple vehicles",
            driveLogs);

        return File(
            pdf,
            "application/pdf",
            $"Driver-{driver.EmployeeId}-DriveLog.pdf");
    }

    // =========================
    // VEHICLE PDF
    // =========================

    [HttpGet("vehicle/{vehicleId}/pdf")]
    public async Task<IActionResult> GetVehiclePdf(int vehicleId)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        var driveLogs = await (
            from drive in _context.DriveLogs
            join user in _context.Users
                on drive.UserId equals user.Id
            where drive.VehicleId == vehicleId
                && drive.EndTime != null
            orderby drive.StartTime descending
            select new DriveLogDto
            {
                Id = drive.Id,
                UserId = drive.UserId,
                VehicleId = drive.VehicleId,

                StartTime = drive.StartTime,
                EndTime = drive.EndTime,

                StartLocation = drive.StartLocation,
                EndLocation = drive.EndLocation,

                Customer = drive.Customer,
                DeliveryNoteId = drive.DeliveryNoteId,

                StartKm = drive.StartKm,
                EndKm = drive.EndKm,

                Purpose = drive.Purpose,

                DriverName = user.FirstName + " " + user.LastName
            }
        ).ToListAsync();

        var pdf = _pdfService.GenerateReport(
            "Vehicle Drive Report",
            "Multiple drivers",
            $"{vehicle.RegistrationNumber} - {vehicle.Make} {vehicle.Model}",
            driveLogs);

        return File(
            pdf,
            "application/pdf",
            $"Vehicle-{vehicle.RegistrationNumber}-DriveLog.pdf");
    }
}