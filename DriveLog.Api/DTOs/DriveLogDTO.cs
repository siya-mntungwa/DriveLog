namespace DriveLog.Api.DTOs;

public class DriveLogDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int VehicleId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string StartLocation { get; set; } = string.Empty;

    public string? EndLocation { get; set; }

    public string Purpose { get; set; } = string.Empty;
}