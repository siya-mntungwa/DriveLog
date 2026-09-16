namespace DriveLog.Api.DTOs;

public class DriverDto
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string  FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}