using DriveLog.Data.Enums;

namespace DriveLog.Api.DTOs;

public class CreateVehicleDto
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}