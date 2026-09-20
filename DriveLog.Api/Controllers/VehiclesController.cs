using DriveLog.Data.Data;
using DriveLog.Data.Models;
using DriveLog.Data.Enums;
using DriveLog.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriveLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly DriveLogDbContext _context;

    public VehiclesController(DriveLogDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetVehicles()
    {
        var vehicles = await _context.Vehicles.ToListAsync();
        return Ok(vehicles);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVehicle(CreateVehicleDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RegistrationNumber))
        {
            return BadRequest("Registration number is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Make))
        {
            return BadRequest("Vehicle make is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Model))
        {
            return BadRequest("Vehicle model is required.");
        }

        if (request.Year < 1900 || request.Year > DateTime.UtcNow.Year + 1)
        {
            return BadRequest("Invalid vehicle year.");
        }

        var registrationExists = await _context.Vehicles
            .AnyAsync(v =>
                v.RegistrationNumber == request.RegistrationNumber);

        if (registrationExists)
        {
            return BadRequest("A vehicle with this registration number already exists.");
        }

        var vehicle = new Vehicle
        {
            RegistrationNumber = request.RegistrationNumber,
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            Status = VehicleStatus.Available
        };

        _context.Vehicles.Add(vehicle);

        await _context.SaveChangesAsync();

        return Ok(vehicle);
    }

    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> DeactivateVehicle(int id)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        if (vehicle.Status == VehicleStatus.InUse)
        {
            return BadRequest("Vehicle cannot be deactivated while it is in use.");
        }

        if (vehicle.Status == VehicleStatus.Inactive)
        {
            return BadRequest("Vehicle is already inactive.");
        }

        vehicle.Status = VehicleStatus.Inactive;

        await _context.SaveChangesAsync();

        return Ok("Vehicle deactivated successfully.");
    }

    [HttpPut("{id}/reactivate")]
    public async Task<IActionResult> ReactivateVehicle(int id)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        if (vehicle.Status != VehicleStatus.Inactive)
        {
            return BadRequest("Only inactive vehicles can be reactivated.");
        }

        vehicle.Status = VehicleStatus.Available;

        await _context.SaveChangesAsync();

        return Ok("Vehicle reactivated successfully.");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(
        int id,
        UpdateVehicleDto request)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        if (string.IsNullOrWhiteSpace(request.RegistrationNumber) ||
            string.IsNullOrWhiteSpace(request.Make) ||
            string.IsNullOrWhiteSpace(request.Model))
        {
            return BadRequest("All vehicle fields are required.");
        }

        if (request.Year < 1900 || request.Year > DateTime.UtcNow.Year + 1)
        {
            return BadRequest("Invalid vehicle year.");
        }

        var registrationExists = await _context.Vehicles
            .AnyAsync(v =>
                v.Id != id &&
                v.RegistrationNumber == request.RegistrationNumber);

        if (registrationExists)
        {
            return BadRequest(
                "A vehicle with this registration number already exists.");
        }

        vehicle.RegistrationNumber = request.RegistrationNumber;
        vehicle.Make = request.Make;
        vehicle.Model = request.Model;
        vehicle.Year = request.Year;

        await _context.SaveChangesAsync();

        return Ok(vehicle);
    }
}