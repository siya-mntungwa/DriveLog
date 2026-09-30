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

    // ==========================================
    // GET ALL DRIVER DOCUMENTS
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> GetDriverDocuments()
    {
        var driverDocuments = await _context.DriveDocuments
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

    // ==========================================
    // GET DOCUMENTS FOR A DRIVER
    // ==========================================

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetDriverDocumentsByDriver(int userId)
    {
        var driverDocuments = await _context.DriveDocuments
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.IsCurrent)
            .ThenByDescending(d => d.ExpiryDate)
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

    // ==========================================
    // UPLOAD DRIVER DOCUMENT
    // ==========================================

    [HttpPost("{userId}/upload")]
    public async Task<IActionResult> UploadDocument(
        int userId,
        [FromForm] UploadDriverDocumentDto request)
    {
        // ------------------------------------------
        // Check driver
        // ------------------------------------------

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

        // ------------------------------------------
        // Validate file
        // ------------------------------------------

        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("A document file is required.");
        }

        // ------------------------------------------
        // Validate dates
        // ------------------------------------------

        if (request.ExpiryDate < request.IssueDate)
        {
            return BadRequest(
                "Expiry date cannot be before issue date.");
        }

        // ------------------------------------------
        // Find current document of same type
        // ------------------------------------------

        var existingCurrentDocument =
            await _context.DriveDocuments
                .FirstOrDefaultAsync(d =>
                    d.UserId == userId &&
                    d.DocumentType == request.DocumentType &&
                    d.IsCurrent);

        // ------------------------------------------
        // Mark old document as no longer current
        // ------------------------------------------

        if (existingCurrentDocument != null)
        {
            existingCurrentDocument.IsCurrent = false;
        }

        // ------------------------------------------
        // Save uploaded file
        // ------------------------------------------

        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            "DriverDocuments");

        Directory.CreateDirectory(uploadsFolder);

        var fileName =
            $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";

        var filePath = Path.Combine(
            uploadsFolder,
            fileName);

        await using (var stream = new FileStream(
            filePath,
            FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }

        // ------------------------------------------
        // Create new document
        // ------------------------------------------

        var document = new DriverDocument
        {
            UserId = driver.Id,
            DocumentType = request.DocumentType,
            FileName = request.File.FileName,
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate,

            // Documents are automatically approved
            DocumentStatus = DocumentStatus.Approved,

            // New document becomes the current document
            IsCurrent = true
        };

        _context.DriveDocuments.Add(document);

        await _context.SaveChangesAsync();

        // ------------------------------------------
        // Return uploaded document
        // ------------------------------------------

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