namespace DriveLog.Api.DTOs;

public class DriverDocumentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string DocumentStatus { get; set; } = string.Empty;
    public bool IsCurrent { get; set; }
}