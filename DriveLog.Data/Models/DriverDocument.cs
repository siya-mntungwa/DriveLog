using DriveLog.Data.Enums;

namespace DriveLog.Data.Models;

public class DriverDocument
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DocumentStatus DocumentStatus { get; set; }
    public bool IsCurrent { get; set; }
}