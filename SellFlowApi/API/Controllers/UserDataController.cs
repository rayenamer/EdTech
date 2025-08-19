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
using API.Helpers;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserDataController : ControllerBase
{
    private readonly IUserDataRepository _UserDataRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserDataController> _logger;

    public UserDataController(IUserDataRepository UserDataRepository, IMapper mapper, ILogger<UserDataController> logger)
    {
        _UserDataRepository = UserDataRepository;
        _mapper = mapper;
        _logger = logger;
    }

    // all apps for admin working good
    [HttpGet("get-all-UserDatas")]
    public async Task<ActionResult<IEnumerable<UserDataDto>>> GetAll()
    {
        try
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
                // Include documents with their names and download URLs
                Documents = app.Documents?.Select(doc => new DocumentDto
                {
                    Id = doc.Id,
                    UserDataId = doc.UserDataId,
                    DocumentName = doc.DocumentName,
                    DownloadUrl = Url.Action("DownloadDocument", "Document", new { id = doc.Id }, Request.Scheme)
                }).ToList() ?? new List<DocumentDto>()
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
                // Include documents with download links
                Documents = app.Documents?.Select(doc => new DocumentDto
                {
                    Id = doc.Id,
                    UserDataId = doc.UserDataId,
                    // Add download link property
                    DownloadUrl = Url.Action("DownloadDocument", "Document", new { id = doc.Id }, Request.Scheme)
                }).ToList() ?? new List<DocumentDto>()
            }).ToList();

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

        var userData = await _UserDataRepository.GetByUserIdAsync(userId);

        if (userData == null || !userData.Any())
        {
            return Ok(new { hasData = false, message = "No user data found." });
        }

        var userDataRecord = userData.First(); // Assuming one record per user

        // Check if required profile fields are completed
        bool hasRequiredInfo = !string.IsNullOrWhiteSpace(userDataRecord.FullName) &&
                              !string.IsNullOrWhiteSpace(userDataRecord.Number) &&
                              userDataRecord.DateOfBirth != default(DateTime) &&
                              !string.IsNullOrWhiteSpace(userDataRecord.Motivation) &&
                              !string.IsNullOrWhiteSpace(userDataRecord.LifeOutSide);

        // Check if user has at least 2 documents
        bool hasMinimumDocuments = userDataRecord.Documents != null &&
                                  userDataRecord.Documents.Count >= 2;

        bool profileCompleted = hasRequiredInfo && hasMinimumDocuments;

        return Ok(new { hasData = profileCompleted });
    }

    // Helper method to get user ID from claims (works with both JWT and OAuth)
    private async Task<(bool success, int userId, IActionResult error)> GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return (false, 0, Unauthorized("User identifier claim not found."));

        // Try to parse the claim as an integer (works for classic JWT auth)
        if (int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogInformation($"Found numeric user ID: {userId}");
            return (true, userId, null);
        }

        // For Google OAuth users, find the user by email
        _logger.LogInformation($"Non-integer user ID found: {userIdClaim}. This might be a Google OAuth user.");

        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("No email claim found for OAuth user");
            return (false, 0, BadRequest("Could not identify user from claims."));
        }

        // Look up the user in the database by email
        var userManager = HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Identity.UserManager<API.Entities.AppUser>))
            as Microsoft.AspNetCore.Identity.UserManager<API.Entities.AppUser>;

        if (userManager == null)
        {
            _logger.LogError("Failed to get UserManager service");
            return (false, 0, StatusCode(500, "Internal server error"));
        }

        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser == null)
        {
            _logger.LogWarning($"No user found with email: {email}");
            return (false, 0, BadRequest("User not found."));
        }

        userId = appUser.Id;
        _logger.LogInformation($"Found user ID {userId} for email {email}");
        return (true, userId, null);
    }

    [HttpPost("add/update-personal-information")]
    public async Task<IActionResult> AddOrUpdatePersonalInformation(PersonalInformationDto personalInfoDto)
    {
        try
        {
            var userIdResult = await GetUserIdFromClaims();
            if (!userIdResult.success)
                return userIdResult.error;

            int userId = userIdResult.userId;
            _logger.LogInformation($"Processing request for UserId: {userId}");
            _logger.LogInformation($"PersonalInfoDto: FullName={personalInfoDto.FullName}, Number={personalInfoDto.Number}, DateOfBirth={personalInfoDto.DateOfBirth}");

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
                _logger.LogInformation("Updating existing UserData record...");
                existingUserData.FullName = personalInfoDto.FullName;
                existingUserData.Number = personalInfoDto.Number;

                // Si DateOfBirth est null, conservez la valeur existante au lieu d'utiliser DateTime.MinValue
                if (personalInfoDto.DateOfBirth.HasValue)
                {
                    existingUserData.DateOfBirth = personalInfoDto.DateOfBirth.Value;
                }

                existingUserData.LinkedinLink = personalInfoDto.LinkedinLink;

                var updateResult = await _UserDataRepository.UpdateAsync(existingUserData);
                Console.WriteLine($"Update result: {updateResult}");
            }
            else
            {
                // Create new record if none exists
                _logger.LogInformation("Creating new UserData record...");
                var newUserData = _mapper.Map<UserData>(personalInfoDto);
                newUserData.UserId = userId;

                // Utiliser une date par défaut seulement si DateOfBirth est null
                if (personalInfoDto.DateOfBirth.HasValue)
                {
                    newUserData.DateOfBirth = personalInfoDto.DateOfBirth.Value;
                }
                else
                {
                    // Utiliser une date par défaut raisonnable (aujourd'hui) au lieu de DateTime.MinValue
                    newUserData.DateOfBirth = DateTime.Today;
                }

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
    [HttpPost("add/update-personal-statements")]
    public async Task<IActionResult> AddOrUpdatePersonalStatements(PersonalStatementsDto personalStatementsDto)
    {
        try
        {
            var userIdResult = await GetUserIdFromClaims();
            if (!userIdResult.success)
                return userIdResult.error;

            int userId = userIdResult.userId;
            _logger.LogInformation($"Processing request for UserId: {userId}");
            _logger.LogInformation($"PersonalStatementsDto: Motivation={personalStatementsDto.Motivation}, LifeOutside={personalStatementsDto.LifeOutSide}");

            var userDatas = await _UserDataRepository.GetByUserIdAsync(userId);
            var existingUserData = userDatas.FirstOrDefault();

            Console.WriteLine($"Found {userDatas.Count()} existing UserData records for UserId: {userId}");
            if (existingUserData != null)
            {
                Console.WriteLine($"Existing UserData ID: {existingUserData.Id}, Motivation: {existingUserData.Motivation}, LifeOutside: {existingUserData.LifeOutSide}");
            }

            if (existingUserData != null)
            {
                // Update existing record
                Console.WriteLine("Updating existing UserData record...");
                existingUserData.Motivation = personalStatementsDto.Motivation;
                existingUserData.LifeOutSide = personalStatementsDto.LifeOutSide;

                var updateResult = await _UserDataRepository.UpdateAsync(existingUserData);
                Console.WriteLine($"Update result: {updateResult}");
            }
            else
            {
                // Create new record if none exists
                Console.WriteLine("Creating new UserData record...");
                var newUserData = _mapper.Map<UserData>(personalStatementsDto);
                newUserData.UserId = userId;
                await _UserDataRepository.AddAsync(newUserData);
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            // Log the exception details
            Console.WriteLine($"Error in AddOrUpdatePersonalStatements: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }

            return BadRequest($"Error updating personal statements: {ex.Message}");
        }
    }
    [HttpPost("add/update-education-background")]
    public async Task<IActionResult> AddOrUpdateEducationBackground(EducationBackgroundDto educationBackgroundDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User identifier claim not found.");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("User identifier claim is not a valid integer.");

            Console.WriteLine($"Processing request for UserId: {userId}");
            Console.WriteLine($"EducationBackgroundDto: {JsonConvert.SerializeObject(educationBackgroundDto)}");

            var userDatas = await _UserDataRepository.GetByUserIdAsync(userId);
            var existingUserData = userDatas.FirstOrDefault();

            Console.WriteLine($"Found {userDatas.Count()} existing UserData records for UserId: {userId}");
            if (existingUserData != null)
            {
                Console.WriteLine($"Existing UserData ID: {existingUserData.Id}");
            }

            if (existingUserData != null)
            {
                // Update existing record
                Console.WriteLine("Updating existing UserData record...");
                existingUserData.BaccalaureatDegree = educationBackgroundDto.BaccalaureatDegree;
                existingUserData.BaccalaureatInstitution = educationBackgroundDto.BaccalaureatInstitution;
                existingUserData.BaccalaureatDate = educationBackgroundDto.BaccalaureatDate;
                existingUserData.BachelorDegree = educationBackgroundDto.BachelorDegree;
                existingUserData.BachelorInstitution = educationBackgroundDto.BachelorInstitution;
                existingUserData.BachelorDate = educationBackgroundDto.BachelorDate;
                existingUserData.MasterDegree = educationBackgroundDto.MasterDegree;
                existingUserData.MasterInstitution = educationBackgroundDto.MasterInstitution;
                existingUserData.MasterDate = educationBackgroundDto.MasterDate;
                existingUserData.EngDegree = educationBackgroundDto.EngDegree;
                existingUserData.EngInstitution = educationBackgroundDto.EngInstitution;
                existingUserData.EngDate = educationBackgroundDto.EngDate;

                var updateResult = await _UserDataRepository.UpdateAsync(existingUserData);
                Console.WriteLine($"Update result: {updateResult}");
            }
            else
            {
                // Create new record if none exists
                Console.WriteLine("Creating new UserData record...");
                var newUserData = _mapper.Map<UserData>(educationBackgroundDto);
                newUserData.UserId = userId;
                await _UserDataRepository.AddAsync(newUserData);
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            // Log the exception details
            Console.WriteLine($"Error in AddOrUpdateEducationBackground: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }

            return BadRequest($"Error updating education background: {ex.Message}");
        }
    }
    [HttpPost("add/update-work-experience")]
    public async Task<IActionResult> AddOrUpdateWorkExperience(WorkExperienceDto workExperienceDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User identifier claim not found.");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("User identifier claim is not a valid integer.");

            Console.WriteLine($"Processing request for UserId: {userId}");
            Console.WriteLine($"WorkExperienceDto: {JsonConvert.SerializeObject(workExperienceDto)}");

            var userDatas = await _UserDataRepository.GetByUserIdAsync(userId);
            var existingUserData = userDatas.FirstOrDefault();

            Console.WriteLine($"Found {userDatas.Count()} existing UserData records for UserId: {userId}");
            if (existingUserData != null)
            {
                Console.WriteLine($"Existing UserData ID: {existingUserData.Id}");
            }

            if (existingUserData != null)
            {
                // Update existing record
                Console.WriteLine("Updating existing UserData record...");
                existingUserData.WorkExperience = workExperienceDto.WorkExperience;

                var updateResult = await _UserDataRepository.UpdateAsync(existingUserData);
                Console.WriteLine($"Update result: {updateResult}");
            }
            else
            {
                // Create new record if none exists
                Console.WriteLine("Creating new UserData record...");
                var newUserData = _mapper.Map<UserData>(workExperienceDto);
                newUserData.UserId = userId;
                await _UserDataRepository.AddAsync(newUserData);
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            // Log the exception details
            Console.WriteLine($"Error in AddOrUpdateWorkExperience: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }

            return BadRequest($"Error updating work experience: {ex.Message}");
        }
    }

}