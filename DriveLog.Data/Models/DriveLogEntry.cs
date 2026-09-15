namespace DriveLog.Data.Models;

public class DriveLogEntry
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string StartLocation { get; set; } = string.Empty;
    public string? EndLocation { get; set; }
    public string purpose { get; set; } = string.Empty;

}