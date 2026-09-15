using DriveLog.Data.Enums;

namespace DriveLog.Data.Models;

public class Vehicle
{
    public int Id { get; set; }

    public string RegistrationNumber { get; set; } = string.Empty;

    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public VehicleStatus Status { get; set; }
}