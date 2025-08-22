
using API.Data;
using API.interfaces;
using API.Dtos;
using API.entities;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUniProgramRepository _uniprogramRepository;
    private readonly ILogger<ApplicationController> _logger;
    private readonly DataContext _context; // Add this

    public ApplicationController(
        IApplicationRepository applicationRepository,
        UserManager<AppUser> userManager,
        IUniProgramRepository uniprogramRepository,
        ILogger<ApplicationController> logger,
        DataContext context) // Add this parameter
    {
        _applicationRepository = applicationRepository;
        _userManager = userManager;
        _uniprogramRepository = uniprogramRepository;
        _logger = logger;
        _context = context; // Add this assignment
    }


    [HttpPost("add-application/{programId}")]
    public async Task<IActionResult> AddApplication(int programId, ApplicationDto applicationDto)
    {
        if (applicationDto == null)
        {
            _logger.LogWarning("Received null application object");
            return BadRequest("Application data is required.");
        }

        var userIdResult = await GetUserIdFromClaims();
        if (!userIdResult.success)
        {
            _logger.LogWarning("Failed to get user ID from claims: {Error}", userIdResult.error);
            return userIdResult.error;
        }

        var applicationData = await GetDataForApplication(userIdResult.userId, programId);

        if (!applicationData.success)
        {
            _logger.LogWarning("Failed to retrieve application data: {ErrorMessage}", applicationData.errorMessage);

            if (applicationData.errorMessage.Contains("User not found"))
            {
                return BadRequest("Please complete your profile before applying");
            }
            if (applicationData.errorMessage.Contains("Program not found"))
            {
                return NotFound("The selected program is not available");
            }

            return StatusCode(500, applicationData.errorMessage);
        }

        try
        {
            // Create Application entity directly (no need for anonymous object or DTO modification)
            var application = new Application
            {
                // From retrieved application data
                StudentName = applicationData.data.StudentName,
                StudentEmail = applicationData.data.StudentEmail,
                StudentContactNumber = applicationData.data.StudentContactNumber,
                ProgramName = applicationData.data.ProgramName,
                ProgramDescription = applicationData.data.ProgramDescription,

                // From DTO
                WhyDidYouApply = applicationDto.WhyDidYouApply,

                // Set default values for required fields
                ApplicationStatus = "Submitted",
                SubmissionDate = DateTime.UtcNow
            };

            var createdApplication = await _applicationRepository.CreateApplicationAsync(application, userIdResult.userId, programId);

            _logger.LogInformation("Application created successfully for user {UserId}, program {ProgramId}", userIdResult.userId, programId);

            return Ok(new
            {
                Message = "Application created successfully",
                ApplicationId = createdApplication.ApplicationId
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already applied"))
        {
            _logger.LogWarning("Duplicate application attempt for user {UserId}, program {ProgramId}", userIdResult.userId, programId);
            return BadRequest(ex.Message); // This will return "You already applied for this program"
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application for user {UserId}, program {ProgramId}", userIdResult.userId, programId);
            return StatusCode(500, "Internal server error while creating application.");
        }
    }

    [HttpGet("get-all-application-with-all-data")]
    public async Task<IActionResult> GetAllApplications()
    {
        try
        {
            var applications = await _applicationRepository.GetAllApplicationsAsync();
            return Ok(applications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all applications");
            return StatusCode(500, "Internal server error while retrieving applications.");
        }
    }


















    private async Task<(bool success, ApplicationData data, string errorMessage)> GetDataForApplication(int userId, int uniProgramId)
    {
        try
        {
            // Load user with UserData included
            var currentUser = await _context.Users
                .Include(u => u.UserData)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (currentUser?.UserData == null)
            {
                return (false, null, "User not found or incomplete profile");
            }

            var chosenProgram = await _uniprogramRepository.GetByIdAsync(uniProgramId);
            if (chosenProgram == null)
            {
                return (false, null, "Program not found");
            }

            var data = new ApplicationData
            {
                StudentName = currentUser.UserData.FullName ?? "N/A",
                StudentEmail = currentUser.Email ?? "N/A",
                StudentContactNumber = currentUser.UserData.Number ?? "N/A",
                ProgramName = chosenProgram.Name ?? "N/A",
                ProgramDescription = chosenProgram.Description ?? "N/A"
            };

            return (true, data, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application data for user {UserId}, program {ProgramId}", userId, uniProgramId);
            return (false, null, "Internal server error");
        }
    }

    [HttpGet("test-get-data/{userId}/{programId}")]
    public async Task<IActionResult> TestGetDataForApplication(int userId, int programId)
    {
        var result = await GetDataForApplication(userId, programId);

        if (!result.success)
        {
            return BadRequest(new
            {
                Success = false,
                ErrorMessage = result.errorMessage,
                UserId = userId,
                ProgramId = programId
            });
        }

        return Ok(new
        {
            Success = true,
            Data = new
            {
                StudentName = result.data.StudentName,
                StudentEmail = result.data.StudentEmail,
                StudentContactNumber = result.data.StudentContactNumber,
                ProgramName = result.data.ProgramName,
                ProgramDescription = result.data.ProgramDescription
            },
            UserId = userId,
            ProgramId = programId,
            Message = "Application data retrieved successfully"
        });
    }


    [HttpGet("test-id-method")]
    public async Task<IActionResult> TestGetUserIdFromClaims()
    {
        var result = await GetUserIdFromClaims();

        if (!result.success)
        {
            return result.error; // This will be Unauthorized, BadRequest, or StatusCode(500)
        }

        return Ok(new
        {
            Success = true,
            UserId = result.userId,
            Message = "User ID retrieved successfully"
        });
    }

    // Keep the original private method for internal use
    private async Task<(bool success, int userId, IActionResult error)> GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return (false, 0, Unauthorized("User identifier claim not found."));

        // Try to parse the claim as an integer (works for classic JWT auth)
        if (int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogInformation("Found numeric user ID: {UserId}", userId);
            return (true, userId, null);
        }

        // For Google OAuth users, find the user by email
        _logger.LogInformation("Non-integer user ID found: {UserIdClaim}. This might be a Google OAuth user.", userIdClaim);

        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("No email claim found for OAuth user");
            return (false, 0, BadRequest("Could not identify user from claims."));
        }

        // Use the injected UserManager instead of getting it from HttpContext
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null)
        {
            _logger.LogWarning("No user found with email: {Email}", email);
            return (false, 0, BadRequest("User not found."));
        }

        userId = appUser.Id;
        _logger.LogInformation("Found user ID {UserId} for email {Email}", userId, email);
        return (true, userId, null);
    }

    internal class ApplicationData
    {
        public required string StudentName { get; set; }
        public required string StudentEmail { get; set; }
        public required string StudentContactNumber { get; set; }
        public required string ProgramName { get; set; }
        public required string ProgramDescription { get; set; }
    }



    //messy code huh ? fix it ;p
    [HttpGet("get-applications-for-the-logged-in-user")]
    public async Task<IActionResult> GetApplicationsForLoggedInUser()
    {
        var result = await GetUserIdFromClaims();

        if (!result.success)
        {
            return result.error; // This will be Unauthorized, BadRequest, or StatusCode(500)
        }

        int userId = result.userId;

        // Fetch applications for the user from the database
        var applications = await _context.Applications
            .Where(app => app.UserId == userId)
            .Select(app => new
            {
                ProgramName = app.ProgramName,
                ProgramDescription = app.ProgramDescription,
                ApplicationStatus = app.ApplicationStatus,
                SubmissionDate = app.SubmissionDate
            })
            .ToListAsync();

        return Ok(new
        {
            Applications = applications,
        });
    }

    [HttpPut("change-application-state/{ApplicationId}")]
    public async Task<IActionResult> ChangeApplicationState(int ApplicationId, string NewState)
    {
        try
        {
            var application = await _context.Applications.FindAsync(ApplicationId);
            if (application == null)
            {
                return NotFound(new { message = "Application not found" });
            }

            application.ApplicationStatus = NewState;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Application status updated successfully" });
        }
        catch
        {
            return BadRequest(new { message = "Error updating application status" });
        }
    }

}
