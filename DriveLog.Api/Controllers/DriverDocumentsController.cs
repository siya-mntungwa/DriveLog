using DriveLog.Data.Data;
using DriveLog.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriveLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriverDocumentsController : ControllerBase
{
    private readonly DriveLogDbContext _context;

    public DriverDocumentsController(DriveLogDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDriverDocuments()
    {
        var driverDocuments = await _context.DriveDocuments.Select(d => new DriverDocumentDto
        {
            Id = d.Id,
            UserId = d.UserId,
            DocumentType = d.DocumentType.ToString(),
            FileName = d.FileName,
            IssueDate = d.IssueDate,
            ExpiryDate = d.ExpiryDate,
            DocumentStatus = d.DocumentStatus.ToString(),
            IsCurrent = d.IsCurrent
        }).ToListAsync();

        return Ok(driverDocuments);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetDriverDocumentsByDriver(int userId)
    {
        var driverDocuments = await _context.DriveDocuments
            .Where(d => d.UserId == userId)
            .Select(d => new DriverDocumentDto
            {
                Id = d.Id,
                UserId = d.UserId,
                DocumentType = d.DocumentType.ToString(),
                FileName = d.FileName,
                IssueDate = d.IssueDate,
                ExpiryDate = d.ExpiryDate,
                DocumentStatus = d.DocumentStatus.ToString(),
                IsCurrent = d.IsCurrent
            })
            .ToListAsync();

        return Ok(driverDocuments);
    }

}