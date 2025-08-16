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
public class UserDataController : ControllerBase
{
    private readonly IUserDataRepository _UserDataRepository;
    private readonly IMapper _mapper;

    public UserDataController(IUserDataRepository UserDataRepository, IMapper mapper)
    {
        _UserDataRepository = UserDataRepository;
        _mapper = mapper;
    }

    // all apps for admin working good
    [HttpGet("get-all-UserDatas")]
    public async Task<ActionResult<IEnumerable<UserDataDto>>> GetAll()
    {
        var UserDatas = await _UserDataRepository.GetAllAsync();

        // Map UserDatas to DTOs with documents included (without content for efficiency)
        var UserDataDtos = UserDatas.Select(app => new UserDataDto
        {
            Id = app.Id,
            FullName = app.FullName,
            Number = app.Number,
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
            Documents = app.Documents.Select(doc => new DocumentDto
            {
                Id = doc.Id,
                Name = doc.Name,
                UploadDate = doc.UploadDate,
                DocumentType = doc.DocumentType,
                Content = Array.Empty<byte>(), // Don't include content in list view for performance
                UserDataId = doc.UserDataId
            }).ToList()
        });

        return Ok(UserDataDtos);
    }

    // GET: api/UserData/get-user-UserDatas
    [HttpGet("get-user-UserDatas")]
    public async Task<ActionResult<UserDataDto>> GetUserUserDatas()
    {
        try
        {
            // Get current user ID
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User identifier claim not found.");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("User identifier claim is not a valid integer.");

            // Get UserDatas for the current user
            var UserDatas = await _UserDataRepository.GetByUserIdAsync(userId);

            // Map UserDatas to DTOs
            var UserDataDtos = UserDatas.Select(app => new UserDataDto
            {
                Id = app.Id,
                FullName = app.FullName,
                Number = app.Number,
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
                Documents = app.Documents.Select(doc => new DocumentDto
                {
                    Id = doc.Id,
                    Name = doc.Name,
                    UploadDate = doc.UploadDate,
                    DocumentType = doc.DocumentType,
                    Content = Array.Empty<byte>(), // Don't include content in list view for performance
                    UserDataId = doc.UserDataId
                }).ToList()
            });

            return Ok(UserDataDtos);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting user UserDatas: {ex.Message}");
        }
    }

    // GET: api/UserData/get-UserData-by-id/5
    [HttpGet("get-UserData-by-id/{id}")]
    public async Task<ActionResult<UserDataDto>> GetById(int id)
    {
        var UserData = await _UserDataRepository.GetByIdAsync(id);
        if (UserData == null)
            return NotFound("UserData not found.");

        // Map UserData to DTO with documents included
        var UserDataDto = new UserDataDto
        {
            Id = UserData.Id,
            FullName = UserData.FullName,
            Number = UserData.Number,
            DateOfBirth = UserData.DateOfBirth,
            Motivation = UserData.Motivation,
            LifeOutSide = UserData.LifeOutSide,
            BaccalaureatDegree = UserData.BaccalaureatDegree,
            BaccalaureatInstitution = UserData.BaccalaureatInstitution,
            BaccalaureatDate = UserData.BaccalaureatDate,
            BachelorDegree = UserData.BachelorDegree,
            BachelorInstitution = UserData.BachelorInstitution,
            BachelorDate = UserData.BachelorDate,
            MasterDegree = UserData.MasterDegree,
            MasterInstitution = UserData.MasterInstitution,
            MasterDate = UserData.MasterDate,
            EngDegree = UserData.EngDegree,
            EngInstitution = UserData.EngInstitution,
            EngDate = UserData.EngDate,
            WorkExperience = UserData.WorkExperience,
            LinkedinLink = UserData.LinkedinLink,
            UserId = UserData.UserId,
            Documents = UserData.Documents.Select(doc => new DocumentDto
            {
                Id = doc.Id,
                Name = doc.Name,
                UploadDate = doc.UploadDate,
                DocumentType = doc.DocumentType,
                Content = doc.Content, // This will be the byte array for download
                UserDataId = doc.UserDataId
            }).ToList()
        };

        return Ok(UserDataDto);
    }

    // POST: api/UserData/add-UserData (with file upload)
    [HttpPost("add-UserData")]
    public async Task<ActionResult<UserData>> Add([FromForm] UserDataDto UserDataDto, [FromForm] List<IFormFile>? files = null)
    {
        try
        {
            // Set UserId from authenticated user
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User identifier claim not found.");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("User identifier claim is not a valid integer.");

            UserDataDto.UserId = userId;

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

                    UserDataDto.Documents.Add(documentDto);
                }
            }

            // Map DTO to entity
            var UserData = _mapper.Map<UserData>(UserDataDto);

            // Save UserData with documents
            var createdUserData = await _UserDataRepository.AddAsync(UserData);
            return CreatedAtAction(nameof(GetById), new { id = createdUserData.Id }, createdUserData);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error creating UserData: {ex.Message}");
        }
    }

    // DELETE: api/UserData/delete-UserData/5
    [HttpDelete("delete-UserData/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _UserDataRepository.DeleteAsync(id);
        if (!deleted)
            return NotFound("UserData not found.");
        return NoContent();
    }
    [HttpGet("download-document/{documentId}")]
    public async Task<IActionResult> DownloadDocument(int documentId)
    {
        try
        {
            // Get the document from the database
            var document = await _UserDataRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
                return NotFound("Document not found.");

            // Determine content type based on file extension
            var extension = Path.GetExtension(document.Name).ToLower();
            var contentType = extension switch
            {
                ".pdf" => "UserData/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "UserData/octet-stream"
            };

            // Return the file as a downloadable response
            return File(document.Content, contentType, document.Name);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error downloading document: {ex.Message}");
        }
    }
    [HttpGet("check-user-has-data")]
    public async Task<IActionResult> CheckUserHasData()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("User identifier claim not found.");

        if (!int.TryParse(userIdClaim, out int userId))
            return BadRequest("User identifier claim is not a valid integer.");

        var userDatas = await _UserDataRepository.GetByUserIdAsync(userId);
        bool hasData = userDatas.Any(); // true if any UserData exists
        return Ok(new { hasData });

    }

    [HttpPost("add/update-personal-information")]
    public async Task<IActionResult> AddOrUpdatePersonalInformation(PersonalInformationDto personalInfoDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User identifier claim not found.");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("User identifier claim is not a valid integer.");

            Console.WriteLine($"Processing request for UserId: {userId}");
            Console.WriteLine($"PersonalInfoDto: FullName={personalInfoDto.FullName}, Number={personalInfoDto.Number}, DateOfBirth={personalInfoDto.DateOfBirth}");

            var userDatas = await _UserDataRepository.GetByUserIdAsync(userId);
            var existingUserData = userDatas.FirstOrDefault();

            Console.WriteLine($"Found {userDatas.Count()} existing UserData records for UserId: {userId}");
            if (existingUserData != null)
            {
                Console.WriteLine($"Existing UserData ID: {existingUserData.Id}, FullName: {existingUserData.FullName}");
            }

            if (existingUserData != null)
            {
                // Update existing record
                Console.WriteLine("Updating existing UserData record...");
                existingUserData.FullName = personalInfoDto.FullName;
                existingUserData.Number = personalInfoDto.Number;
                existingUserData.DateOfBirth = personalInfoDto.DateOfBirth ?? DateTime.MinValue;
                existingUserData.LinkedinLink = personalInfoDto.LinkedinLink;
                
                var updateResult = await _UserDataRepository.UpdateAsync(existingUserData);
                Console.WriteLine($"Update result: {updateResult}");
            }
            else
            {
                // Create new record if none exists
                Console.WriteLine("Creating new UserData record...");
                var newUserData = _mapper.Map<UserData>(personalInfoDto);
                newUserData.UserId = userId;
                newUserData.DateOfBirth = personalInfoDto.DateOfBirth ?? DateTime.MinValue;
                await _UserDataRepository.AddAsync(newUserData);
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            // Log the exception details
            Console.WriteLine($"Error in AddOrUpdatePersonalInformation: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            
            return BadRequest($"Error updating personal information: {ex.Message}");
        }
    }




}