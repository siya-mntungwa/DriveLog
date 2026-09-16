namespace DriveLog.Api.DTOs;

public class StartDriveDto
{
    public int UserId { get; set; }

    public int VehicleId { get; set; }

    public string StartLocation { get; set; } = string.Empty;

    public string Purpose { get; set; } = string.Empty;
}