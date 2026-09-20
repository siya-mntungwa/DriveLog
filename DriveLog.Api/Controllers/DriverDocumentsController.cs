using DriveLog.Data.Data;
using DriveLog.Data.Models;
using DriveLog.Api.DTOs;
using DriveLog.Data.Enums;
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

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveDocument(int id)
    {
        var document = await _context.DriveDocuments
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null)
        {
            return NotFound("Document not found.");
        }

        var currentDocuments = await _context.DriveDocuments
            .Where(d =>
                d.UserId == document.UserId &&
                d.DocumentType == document.DocumentType &&
                d.IsCurrent &&
                d.Id != document.Id)
            .ToListAsync();

        foreach (var currentDocument in currentDocuments)
        {
            currentDocument.IsCurrent = false;
        }

        document.DocumentStatus = DocumentStatus.Approved;
        document.IsCurrent = true;

        await _context.SaveChangesAsync();

        return Ok("Document approved successfully.");
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectDocument(int id)
    {
        var document = await _context.DriveDocuments
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null)
        {
            return NotFound("Document not found.");
        }

        document.DocumentStatus = DocumentStatus.Rejected;

        await _context.SaveChangesAsync();

        return Ok("Document rejected successfully.");
    }

    [HttpPost("{userId}/upload")]
    public async Task<IActionResult> UploadDocument(
        int userId,
        [FromForm] UploadDriverDocumentDto request)
    {
        var driver = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (driver == null)
        {
            return NotFound("Driver not found.");
        }

        if (driver.Role != UserRole.Driver)
        {
            return BadRequest("User is not a driver.");
        }

        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("A document file is required.");
        }

        if (request.ExpiryDate < request.IssueDate)
        {
            return BadRequest("Expiry date cannot be before issue date.");
        }

        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            "DriverDocuments");

        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";

        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(
            filePath,
            FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }

        var document = new DriverDocument
        {
            UserId = driver.Id,
            DocumentType = request.DocumentType,
            FileName = request.File.FileName,
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate,
            DocumentStatus = DocumentStatus.Pending,
            IsCurrent = false
        };

        _context.DriveDocuments.Add(document);

        await _context.SaveChangesAsync();

        return Ok(new DriverDocumentDto
        {
            Id = document.Id,
            UserId = document.UserId,
            DocumentType = document.DocumentType.ToString(),
            FileName = document.FileName,
            IssueDate = document.IssueDate,
            ExpiryDate = document.ExpiryDate,
            DocumentStatus = document.DocumentStatus.ToString(),
            IsCurrent = document.IsCurrent
        });
    }

}