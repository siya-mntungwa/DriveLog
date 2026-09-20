using DriveLog.Data.Enums;
using Microsoft.AspNetCore.Http;

namespace DriveLog.Api.DTOs;

public class UploadDriverDocumentDto
{
    public IFormFile File { get; set; } = null!;
    public DocumentType DocumentType { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
}