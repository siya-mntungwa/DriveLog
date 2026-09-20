namespace DriveLog.Api.DTOs;

public class DriverEligibilityDto
{
    public int UserId { get; set; }
    public bool IsEligible { get; set; }
    public bool HasValidDriversLicense { get; set; }
    public bool HasValidPDP { get; set; }
    public string Message { get; set; } = string.Empty;
}