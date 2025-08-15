using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.entities;
using Microsoft.AspNetCore.Authorization;
using API.DATA;
using API.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IMapper _mapper;

    public ApplicationController(IApplicationRepository applicationRepository, IMapper mapper)
    {
        _applicationRepository = applicationRepository;
        _mapper = mapper;
    }

    // all apps for admin working good
    [HttpGet("get-all-applications")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetAll()
    {
        var applications = await _applicationRepository.GetAllAsync();
        
        // Map applications to DTOs with documents included (without content for efficiency)
        var applicationDtos = applications.Select(app => new ApplicationDto
        {
            Id = app.Id,
            FullName = app.FullName,
            Number = app.Number,
            Status = app.Status,
            CreatedAt = app.CreatedAt,
            DateOfBirth = app.DateOfBirth,
            Motivation = app.Motivation,
            LifeOutSide = app.LifeOutSide,
            BaccalaureatDegree = app.BaccalaureatDegree,
            BaccalaureatInstitution = app.BaccalaureatInstitution,
            BaccalaureatDate = app.BaccalaureatDate,
            BachelorDegree = app.BachelorDegree,
            BachelorInstitution = app.BachelorInstitution,
            BachelorDate = app.BachelorDate,
            MasterDegree = app.MasterDegree,
            MasterInstitution = app.MasterInstitution,
            MasterDate = app.MasterDate,
            EngDegree = app.EngDegree,
            EngInstitution = app.EngInstitution,
            EngDate = app.EngDate,
            WorkExperience = app.WorkExperience,
            LinkedinLink = app.LinkedinLink,
            UserId = app.UserId,
            ProgramId = app.ProgramId,
            Documents = app.Documents.Select(doc => new DocumentDto
            {
                Id = doc.Id,
                Name = doc.Name,
                UploadDate = doc.UploadDate,
                DocumentType = doc.DocumentType,
                Content = Array.Empty<byte>(), // Don't include content in list view for performance
                ApplicationId = doc.ApplicationId
            }).ToList()
        });
        
        return Ok(applicationDtos);
    }

    // GET: api/application/get-user-applications
    [HttpGet("get-user-applications")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetUserApplications()
    {
        try
        {
            // Get current user ID
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User identifier claim not found.");
            
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("User identifier claim is not a valid integer.");

            // Get applications for the current user
            var applications = await _applicationRepository.GetByUserIdAsync(userId);
            
            // Map applications to DTOs
            var applicationDtos = applications.Select(app => new ApplicationDto
            {
                Id = app.Id,
                FullName = app.FullName,
                Number = app.Number,
                Status = app.Status,
                CreatedAt = app.CreatedAt,
                DateOfBirth = app.DateOfBirth,
                Motivation = app.Motivation,
                LifeOutSide = app.LifeOutSide,
                BaccalaureatDegree = app.BaccalaureatDegree,
                BaccalaureatInstitution = app.BaccalaureatInstitution,
                BaccalaureatDate = app.BaccalaureatDate,
                BachelorDegree = app.BachelorDegree,
                BachelorInstitution = app.BachelorInstitution,
                BachelorDate = app.BachelorDate,
                MasterDegree = app.MasterDegree,
                MasterInstitution = app.MasterInstitution,
                MasterDate = app.MasterDate,
                EngDegree = app.EngDegree,
                EngInstitution = app.EngInstitution,
                EngDate = app.EngDate,
                WorkExperience = app.WorkExperience,
                LinkedinLink = app.LinkedinLink,
                UserId = app.UserId,
                ProgramId = app.ProgramId,
                Documents = app.Documents.Select(doc => new DocumentDto
                {
                    Id = doc.Id,
                    Name = doc.Name,
                    UploadDate = doc.UploadDate,
                    DocumentType = doc.DocumentType,
                    Content = Array.Empty<byte>(), // Don't include content in list view for performance
                    ApplicationId = doc.ApplicationId
                }).ToList()
            });
            
            return Ok(applicationDtos);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting user applications: {ex.Message}");
        }
    }

    // GET: api/application/get-application-by-id/5
    [HttpGet("get-application-by-id/{id}")]
    public async Task<ActionResult<ApplicationDto>> GetById(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null)
            return NotFound("Application not found.");
            
        // Map application to DTO with documents included
        var applicationDto = new ApplicationDto
        {
            Id = application.Id,
            FullName = application.FullName,
            Number = application.Number,
            Status = application.Status,
            CreatedAt = application.CreatedAt,
            DateOfBirth = application.DateOfBirth,
            Motivation = application.Motivation,
            LifeOutSide = application.LifeOutSide,
            BaccalaureatDegree = application.BaccalaureatDegree,
            BaccalaureatInstitution = application.BaccalaureatInstitution,
            BaccalaureatDate = application.BaccalaureatDate,
            BachelorDegree = application.BachelorDegree,
            BachelorInstitution = application.BachelorInstitution,
            BachelorDate = application.BachelorDate,
            MasterDegree = application.MasterDegree,
            MasterInstitution = application.MasterInstitution,
            MasterDate = application.MasterDate,
            EngDegree = application.EngDegree,
            EngInstitution = application.EngInstitution,
            EngDate = application.EngDate,
            WorkExperience = application.WorkExperience,
            LinkedinLink = application.LinkedinLink,
            UserId = application.UserId,
            ProgramId = application.ProgramId,
            Documents = application.Documents.Select(doc => new DocumentDto
            {
                Id = doc.Id,
                Name = doc.Name,
                UploadDate = doc.UploadDate,
                DocumentType = doc.DocumentType,
                Content = doc.Content, // This will be the byte array for download
                ApplicationId = doc.ApplicationId
            }).ToList()
        };
        
        return Ok(applicationDto);
    }

    // POST: api/application/add-application (with file upload)
    [HttpPost("add-application")]
    public async Task<ActionResult<Application>> Add([FromForm] ApplicationDto applicationDto, [FromForm] List<IFormFile>? files = null)
    {
        try
        {
            // Set UserId from authenticated user
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User identifier claim not found.");
            
            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("User identifier claim is not a valid integer.");
                
            applicationDto.UserId = userId;

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

                    // Get document type from form data or use file extension as fallback
                    var documentType = Request.Form["documentType"].ToString();
                    if (string.IsNullOrEmpty(documentType))
                    {
                        documentType = extension switch
                        {
                            ".pdf" => "PDF Document",
                            ".jpg" or ".jpeg" => "Image",
                            ".png" => "Image",
                            _ => "Unknown"
                        };
                    }

                    // Convert file to byte[]
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();

                    // Create document DTO
                    var documentDto = new DocumentDto
                    {
                        Name = file.FileName,
                        Content = fileBytes,
                        DocumentType = documentType
                    };

                    applicationDto.Documents.Add(documentDto);
                }
            }

            // Map DTO to entity
            var application = _mapper.Map<Application>(applicationDto);

            // Save application with documents
            var createdApplication = await _applicationRepository.AddAsync(application);
            return CreatedAtAction(nameof(GetById), new { id = createdApplication.Id }, createdApplication);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error creating application: {ex.Message}");
        }
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
    [HttpGet("download-document/{documentId}")]
    public async Task<IActionResult> DownloadDocument(int documentId)
    {
        try
        {
            // Get the document from the database
            var document = await _applicationRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
                return NotFound("Document not found.");

            // Determine content type based on file extension
            var extension = Path.GetExtension(document.Name).ToLower();
            var contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            // Return the file as a downloadable response
            return File(document.Content, contentType, document.Name);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error downloading document: {ex.Message}");
        }
    }
}