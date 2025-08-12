using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using API.Data;
using API.entities;
using Microsoft.AspNetCore.Authorization;
using API.DATA;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationRepository _applicationRepository;

    public ApplicationController(IApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    // GET: api/application/get-all-applications
    [HttpGet("get-all-applications")]
    public async Task<ActionResult<IEnumerable<Application>>> GetAll()
    {
        var applications = await _applicationRepository.GetAllAsync();
        return Ok(applications);
    }

    // GET: api/application/get-application-by-id/5
    [HttpGet("get-application-by-id/{id}")]
    public async Task<ActionResult<Application>> GetById(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null)
            return NotFound("Application not found.");
        return Ok(application);
    }

    // POST: api/application/add-application
    [HttpPost("add-application")]
    public async Task<ActionResult<Application>> Add([FromForm] Application application, IFormFileCollection files)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Set UserId from authenticated user
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("User identifier claim not found.");
        application.UserId = int.Parse(userIdClaim);

        // Validate and process files
        if (files != null && files.Count > 0)
        {
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            foreach (var file in files)
            {
                // Validate file
                if (file.Length == 0)
                    return BadRequest($"File {file.FileName} is empty.");

                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                    return BadRequest($"Invalid file type for {file.FileName}. Only PDF, JPG, JPEG, and PNG are allowed.");

                if (file.Length > 10 * 1024 * 1024) // 10MB limit
                    return BadRequest($"File {file.FileName} exceeds 10MB.");

                // Parse document type from form data or file name (adjust as needed)
                var documentTypeStr = file.ContentDisposition
                    .Split(';')
                    .FirstOrDefault(p => p.Trim().StartsWith("name=documentType"))?
                    .Split('=')[1]
                    ?.Trim();

                // Fallback if documentTypeStr is null or empty
                var docType = !string.IsNullOrEmpty(documentTypeStr) ? documentTypeStr : "Unknown";

                // Convert file to byte[]
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                // Create document
                var document = new Document
                {
                    Name = file.FileName,
                    Content = fileBytes,
                    DocumentType = docType
                };

                application.Documents.Add(document);
            }
        }

        // Save application with documents
        var createdApplication = await _applicationRepository.AddAsync(application);
        return CreatedAtAction(nameof(GetById), new { id = createdApplication.Id }, createdApplication);
    }

    // DELETE: api/application/delete-application/5
    [HttpDelete("delete-application/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _applicationRepository.DeleteAsync(id);
        if (!deleted)
            return NotFound("Application not found.");
        return NoContent();
    }

    // GET: api/application/5/documents
    [HttpGet("{applicationId}/documents")]
    public async Task<ActionResult<IEnumerable<Document>>> GetDocuments(int applicationId)
    {
        var application = await _applicationRepository.GetByIdAsync(applicationId);
        if (application == null)
            return NotFound("Application not found.");
        return Ok(application.Documents);
    }
}