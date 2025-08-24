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
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using API.interfaces;

using API.Helpers;
using API.interfaces;
namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserDataController : ControllerBase
{
    private readonly DataContext context;
    private readonly GetUserId _getUserIdHelper;
    private readonly IUserDataRepository _UserDataRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserDataController> _logger;

    public UserDataController(IUserDataRepository UserDataRepository, IMapper mapper, ILogger<UserDataController> logger, GetUserId getUserIdHelper, DataContext context)
    {
        _UserDataRepository = UserDataRepository;
        _mapper = mapper;
        _logger = logger;
        _getUserIdHelper = getUserIdHelper;
        this.context = context;
    }

    // all apps for admin working good
    [HttpGet("get-all-UserDatas")]
    public async Task<ActionResult<IEnumerable<UserDataDto>>> GetAll()
    {
        try
        {
            // Use projection to get document metadata without BLOB data
            var UserDatas = await context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Number,
                    u.UserDataDateOfBirth,
                    u.Motivation,
                    u.LifeOutSide,
                    u.BaccalaureatDegree,
                    u.BaccalaureatInstitution,
                    u.BaccalaureatDate,
                    u.BachelorDegree,
                    u.BachelorInstitution,
                    u.BachelorDate,
                    u.MasterDegree,
                    u.MasterInstitution,
                    u.MasterDate,
                    u.EngDegree,
                    u.EngInstitution,
                    u.EngDate,
                    u.WorkExperience,
                    u.LinkedinLink,
                    Documents = u.Documents.Select(d => new
                    {
                        d.Id,
                        d.UserDataId,
                        d.DocumentName
                        // NO d.Bytes - just metadata!
                    }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            var UserDataDtos = UserDatas.Select(app => new UserDataDto
            {
                Id = app.Id,
                FullName = app.FullName,
                Number = app.Number,
                DateOfBirth = app.UserDataDateOfBirth,
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
                UserId = app.Id,
                Documents = app.Documents.Select(doc => new DocumentDto
                {
                    Id = doc.Id,
                    UserDataId = doc.UserDataId,
                    DocumentName = doc.DocumentName,
                    DownloadUrl = Url.Action("DownloadDocument", "Document", new { id = doc.Id }, Request.Scheme)
                }).ToList()
            });

            return Ok(UserDataDtos);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error getting all UserDatas: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets all UserData records for the current user with document download links
    /// </summary>
    /// <returns>A collection of UserDataDto objects with document download links</returns>
    // GET: api/UserData/get-user-UserDatas
    [HttpGet("get-user-UserDatas")]
    public async Task<ActionResult<IEnumerable<UserDataDto>>> GetUserUserDatas()
    {
        _logger.LogInformation("===========================================");
        _logger.LogInformation("=====Getting My User Profile Starting =====");
        _logger.LogInformation("===========================================");
        _logger.LogInformation("===========================================");
        _logger.LogInformation("===========================================");
        _logger.LogInformation("===========================================");
        _logger.LogInformation("===========================================");
        try
        {

            // Get current user ID
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User identifier claim not found.");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("User identifier claim is not a valid integer.");

            // Get User data for the current user
            var userData = await _UserDataRepository.GetByUserIdAsync(userId);

            if (userData == null)
            {
                return Ok(new List<UserDataDto>());
            }

            // Map User to UserDataDto
            var userDataDto = new UserDataDto
            {
                Id = userData.Id,
                FullName = userData.FullName,
                Number = userData.Number,
                DateOfBirth = userData.UserDataDateOfBirth,
                Motivation = userData.Motivation,
                LifeOutSide = userData.LifeOutSide,
                BaccalaureatDegree = userData.BaccalaureatDegree,
                BaccalaureatInstitution = userData.BaccalaureatInstitution,
                BaccalaureatDate = userData.BaccalaureatDate,
                BachelorDegree = userData.BachelorDegree,
                BachelorInstitution = userData.BachelorInstitution,
                BachelorDate = userData.BachelorDate,
                MasterDegree = userData.MasterDegree,
                MasterInstitution = userData.MasterInstitution,
                MasterDate = userData.MasterDate,
                EngDegree = userData.EngDegree,
                EngInstitution = userData.EngInstitution,
                EngDate = userData.EngDate,
                WorkExperience = userData.WorkExperience,
                LinkedinLink = userData.LinkedinLink,
                UserId = userData.Id,
                // Include documents with download links
                Documents = userData.Documents?.Select(doc => new DocumentDto
                {
                    Id = doc.Id,
                    UserDataId = doc.UserDataId,
                    // Add download link property
                    DownloadUrl = Url.Action("DownloadDocument", "Document", new { id = doc.Id }, Request.Scheme)
                }).ToList() ?? new List<DocumentDto>()
            };

            var UserDataDtos = new List<UserDataDto> { userDataDto };

            return Ok(UserDataDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user UserDatas");
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

        // Map User to UserDataDto with documents included
        var UserDataDto = new UserDataDto
        {
            Id = UserData.Id,
            FullName = UserData.FullName,
            Number = UserData.Number,
            DateOfBirth = UserData.UserDataDateOfBirth,
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
            UserId = UserData.Id,
        };

        return Ok(UserDataDto);
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

    [HttpGet("check-user-has-data")]
    public async Task<IActionResult> CheckUserHasData()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("User identifier claim not found.");

        if (!int.TryParse(userIdClaim, out int userId))
            return BadRequest("User identifier claim is not a valid integer.");

        // Get user data WITHOUT loading document bytes
        var userData = await context.Users
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.FullName,
                u.Number,
                u.UserDataDateOfBirth,
                u.Motivation,
                u.LifeOutSide,
                DocumentCount = u.Documents.Count() // Count without loading bytes
            })
            .FirstOrDefaultAsync();

        if (userData == null)
            return Ok(new { hasData = false, message = "No user data found." });

        bool hasRequiredInfo = !string.IsNullOrWhiteSpace(userData.FullName) &&
                              !string.IsNullOrWhiteSpace(userData.Number) &&
                              userData.UserDataDateOfBirth != default(DateTime) &&
                              !string.IsNullOrWhiteSpace(userData.Motivation) &&
                              !string.IsNullOrWhiteSpace(userData.LifeOutSide);

        bool hasMinimumDocuments = userData.DocumentCount >= 2;
        bool profileCompleted = hasRequiredInfo && hasMinimumDocuments;

        return Ok(new { hasData = profileCompleted });
    }

    // Helper method to get user ID from claims (works with both JWT and OAuth)


    [HttpPost("add/update-personal-information")]
    public async Task<IActionResult> AddOrUpdatePersonalInformation(PersonalInformationDto personalInfoDto)
    {
        _logger.LogInformation("===========================================");
        _logger.LogInformation("======== UpdateInfoDataCalled =============");
        _logger.LogInformation("===========================================");
        try
        {
            var userIdResult = await _getUserIdHelper.GetUserIdFromClaims(User);
            int userId = userIdResult.userId;
            _logger.LogInformation($"Processing request for UserId: {userId}");

            var updateResult = await _UserDataRepository.UpdatePersonalInformationAsync(userId, personalInfoDto);

            if (!updateResult)
            {
                _logger.LogError("User not found for personal information update");
                return BadRequest("User not found");
            }

            Console.WriteLine($"Update result: {updateResult}");
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddOrUpdatePersonalInformation: {ex.Message}");
            return BadRequest($"Error updating personal information: {ex.Message}");
        }
    }

    [HttpPost("add/update-personal-statements")]
    public async Task<IActionResult> AddOrUpdatePersonalStatements(PersonalStatementsDto personalStatementsDto)
    {
         _logger.LogInformation("===========================================");
        _logger.LogInformation("======== Updating Personal Stats =============");
        _logger.LogInformation("===========================================");
        try
        {
            var userIdResult = await _getUserIdHelper.GetUserIdFromClaims(User);
            int userId = userIdResult.userId;
            _logger.LogInformation($"Processing request for UserId: {userId}");

            var updateResult = await _UserDataRepository.UpdatePersonalStatementsAsync(userId, personalStatementsDto);

            if (!updateResult)
            {
                _logger.LogError("User not found for personal statements update");
                return BadRequest("User not found");
            }

            Console.WriteLine($"Update result: {updateResult}");
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddOrUpdatePersonalStatements: {ex.Message}");
            return BadRequest($"Error updating personal statements: {ex.Message}");
        }
    }

    [HttpPost("add/update-education-background")]
    public async Task<IActionResult> AddOrUpdateEducationBackground(EducationBackgroundDto educationBackgroundDto)
    {
         _logger.LogInformation("===========================================");
        _logger.LogInformation("======== Updating education backg =============");
        _logger.LogInformation("===========================================");
        try
        {
            var userIdResult = await _getUserIdHelper.GetUserIdFromClaims(User);
            int userId = userIdResult.userId;
            _logger.LogInformation($"Processing request for UserId: {userId}");

            var updateResult = await _UserDataRepository.UpdateEducationBackgroundAsync(userId, educationBackgroundDto);

            if (!updateResult)
            {
                _logger.LogError("User not found for education background update");
                return BadRequest("User not found");
            }

            Console.WriteLine($"Update result: {updateResult}");
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddOrUpdateEducationBackground: {ex.Message}");
            return BadRequest($"Error updating education background: {ex.Message}");
        }
    }
    
    [HttpPost("add/update-work-experience")]
    public async Task<IActionResult> AddOrUpdateWorkExperience(WorkExperienceDto workExperienceDto)
    {
         _logger.LogInformation("===========================================");
        _logger.LogInformation("======== Updating work eperience =============");
        _logger.LogInformation("===========================================");
        try
        {
            var userIdResult = await _getUserIdHelper.GetUserIdFromClaims(User);
            int userId = userIdResult.userId;
            _logger.LogInformation($"Processing request for UserId: {userId}");

            var updateResult = await _UserDataRepository.UpdateWorkExperienceAsync(userId, workExperienceDto);

            if (!updateResult)
            {
                _logger.LogError("User not found for work experience update");
                return BadRequest("User not found");
            }

            Console.WriteLine($"Update result: {updateResult}");
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddOrUpdateWorkExperience: {ex.Message}");
            return BadRequest($"Error updating work experience: {ex.Message}");
        }
    }

    [HttpGet("get-user-data-with-documents")]
    public async Task<ActionResult<UserDataWithDocumentsDto>> GetUserDataWithDocuments()
    {
        _logger.LogInformation("===========================================");
        _logger.LogInformation("===== Getting User Profile with Documents =====");
        _logger.LogInformation("===========================================");

        try
        {
            // Get current user ID once
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("Invalid user identifier.");
            }

            _logger.LogInformation("Processing request for User ID: {UserId}", userId);

            // SINGLE database call to get user with documents
            var userData = await _UserDataRepository.GetUserWithDocumentsAsync(userId);

            if (userData == null)
            {
                _logger.LogInformation("User not found for ID: {UserId}", userId);
                return NotFound("User data not found.");
            }

            // Create UserDataDto
            var userDataDto = new UserDataDto
            {
                Id = userData.Id,
                FullName = userData.FullName,
                Number = userData.Number,
                DateOfBirth = userData.UserDataDateOfBirth,
                Motivation = userData.Motivation,
                LifeOutSide = userData.LifeOutSide,
                BaccalaureatDegree = userData.BaccalaureatDegree,
                BaccalaureatInstitution = userData.BaccalaureatInstitution,
                BaccalaureatDate = userData.BaccalaureatDate,
                BachelorDegree = userData.BachelorDegree,
                BachelorInstitution = userData.BachelorInstitution,
                BachelorDate = userData.BachelorDate,
                MasterDegree = userData.MasterDegree,
                MasterInstitution = userData.MasterInstitution,
                MasterDate = userData.MasterDate,
                EngDegree = userData.EngDegree,
                EngInstitution = userData.EngInstitution,
                EngDate = userData.EngDate,
                WorkExperience = userData.WorkExperience,
                LinkedinLink = userData.LinkedinLink,
                UserId = userData.Id,
                Documents = userData.Documents?.Select(doc => new DocumentDto
                {
                    Id = doc.Id,
                    UserDataId = doc.UserDataId,
                    DocumentName = doc.DocumentName,
                    DownloadUrl = Url.Action("DownloadDocument", "Document", new { id = doc.Id }, Request.Scheme)
                }).ToList() ?? new List<DocumentDto>()
            };

            // Create document status based on existing documents
            var documentStatus = new DocumentStatusDto
            {
                Cv = userData.Documents?.Any(d => d.DocumentName.Equals("CV", StringComparison.OrdinalIgnoreCase)) ?? false,
                Baccalaureat = userData.Documents?.Any(d => d.DocumentName.Equals("Baccalaureat", StringComparison.OrdinalIgnoreCase)) ?? false,
                BaccalaureatGrades = userData.Documents?.Any(d => d.DocumentName.Equals("BaccalaureatGrades", StringComparison.OrdinalIgnoreCase)) ?? false,
                Bachelor = userData.Documents?.Any(d => d.DocumentName.Equals("Bachelor", StringComparison.OrdinalIgnoreCase)) ?? false,
                BachelorGrades = userData.Documents?.Any(d => d.DocumentName.Equals("BachelorGrades", StringComparison.OrdinalIgnoreCase)) ?? false
            };

            var result = new UserDataWithDocumentsDto
            {
                UserData = userDataDto,
                DocumentStatus = documentStatus
            };

            _logger.LogInformation("Successfully retrieved user data with documents for User ID: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user data with documents");
            return BadRequest($"Error retrieving user data with documents: {ex.Message}");
        }
    }

}