using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using API.Entities;
using System.Security.Claims;

namespace API.Helpers;

public class GetUserId
{
    private readonly ILogger<GetUserId> _logger;
    private readonly UserManager<AppUser> _userManager;

    public GetUserId(ILogger<GetUserId> logger, UserManager<AppUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<(int userId, IActionResult error)> GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return (0, new UnauthorizedObjectResult("User identifier claim not found."));

        // Try to parse the claim as an integer (works for classic JWT auth)
        if (int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogInformation("Found numeric user ID: {UserId}", userId);
            return (userId, null);
        }

        // For Google OAuth users, find the user by email
        _logger.LogInformation("Non-integer user ID found: {UserIdClaim}. This might be a Google OAuth user.", userIdClaim);

        var email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("No email claim found for OAuth user");
            return (0, new UnauthorizedObjectResult("User email claim not found."));
        }

        // Use the injected UserManager instead of getting it from HttpContext
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null)
        {
            _logger.LogWarning("No user found with email: {Email}", email);
            return (0, new NotFoundObjectResult($"User with email {email} not found."));
        }

        userId = appUser.Id;
        _logger.LogInformation("Found user ID {UserId} for email {Email}", userId, email);
        return (userId, null);
    }
}