using System;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.Dtos;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Web;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using API.DATA;
using API.entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Controllers;

public class Register_LoginController
(
    UserManager<AppUser> userManager,
    IUserDataRepository _UserDataRepository,
    ITokenService tokenService,
    IMapper mapper,
    IEmailSender _emailSender,
    ILogger<Register_LoginController> _logger
) : BaseApiController
{
    /*[HttpPost("register")]
    public async Task<ActionResult<AppUserDto>> Register(RegisterDto registerDto)
    {
        var userExist = await userManager.FindByEmailAsync(registerDto.Email);
        if (userExist != null) return BadRequest("Email already registered");

        var user = mapper.Map<AppUser>(registerDto);
        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        var token = await tokenService.CreateToken(user);

        // Set token as HTTP-only cookie
        Response.Cookies.Append("jwt_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return new AppUserDto
        {
            Username = user.UserName ?? "",
            Token = token,
            Gender = user.Gender,
            city = user.city,
            Country = user.Country,
            PhoneNumber = user.PhoneNumber ?? "",
            Email = user.Email ?? "",
        };
    }*/

    [HttpPost("login")]
    public async Task<ActionResult<AppUserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.NormalizedEmail == loginDto.Email.ToUpper());
        if (user == null || user.Email == null)
        {
            return Unauthorized("Invalid email");
        }

        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result) return Unauthorized("Wrong password");

        var token = await tokenService.CreateToken(user);

        // Set token as HTTP-only cookie
        Response.Cookies.Append("jwt_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        user.LastActive = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        return new AppUserDto
        {
            Username = user.UserName ?? "",
            Token = token,
            Gender = user.Gender,
            city = user.city,
            Country = user.Country,
            PhoneNumber = user.PhoneNumber ?? "",
            Email = user.Email ?? "",
        };
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<AppUserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized("User identifier claim not found.");

        AppUser user;
        
        // Try to parse as integer first (for regular JWT users)
        if (int.TryParse(userIdClaim, out int userId))
        {
            user = await userManager.FindByIdAsync(userId.ToString());
        }
        else
        {
            // For Google OAuth users, find by email
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return BadRequest("Could not identify user from claims.");
            
            user = await userManager.FindByEmailAsync(email);
        }

        if (user == null)
            return NotFound("User not found");

        return new AppUserDto
        {
            Username = user.UserName ?? "",
            Token = "", // Don't send token in response for security
            Gender = user.Gender,
            city = user.city,
            Country = user.Country,
            PhoneNumber = user.PhoneNumber ?? "",
            Email = user.Email ?? "",
        };
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt_token");
        return Ok(new { message = "Logged out successfully" });
    }

    //
    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        _logger.LogInformation("====================LOGIN-STARTED===================");
        var properties = new AuthenticationProperties
        {
            RedirectUri = "https://localhost:7030/api/Register_Login/google-response"
        };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse([FromQuery] string state = "")
    {
        // Get configuration for URLs
        var config = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
        var loginFailedUrl = config?["Frontend:LoginFailedUrl"] ?? "https://localhost:4200/login-failed";
        var googleSuccessUrl = config?["Frontend:GoogleSuccessUrl"] ?? "https://localhost:4200/login";

        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded || result.Principal == null)
        {
            _logger.LogWarning("Google authentication failed or principal missing.");
            return Redirect($"{loginFailedUrl}?error=auth_failed");
        }

        var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
        {
            _logger.LogWarning("Google claims missing: email or name.");
            return Redirect($"{loginFailedUrl}?error=missing_claims");
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new AppUser
            {
                Email = email,
                UserName = name.Replace(" ", "").ToLower(),
                EmailConfirmed = true,
                Gender = "",
                city = "",
                Country = "",
                PhoneNumber = "",
            };

            var resultCreate = await userManager.CreateAsync(user);
            if (!resultCreate.Succeeded)
            {
                _logger.LogError("Failed to create user for Google login: {Errors}", resultCreate.Errors);
                return Redirect($"{loginFailedUrl}?error=user_creation_failed");
            }
            try
            {
                // Update user with default UserData values
                user.UserDataExists = "true";
                await _UserDataRepository.UpdateAsync(user);
                _logger.LogInformation("UserData created successfully for Google user {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create UserData for Google user {Email}", email);
                // You might want to decide whether to continue or fail here
                // For now, we'll continue but log the error
            }
        }

        var token = await tokenService.CreateToken(user);

        // Set token as HTTP-only cookie (recommended for security)
        Response.Cookies.Append("jwt_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        _logger.LogInformation("Google login successful for user {Email}", email);
        user.LastActive = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        // Create user data to pass to frontend
        var userData = new
        {
            username = user.UserName ?? "",
            token = token,
            gender = user.Gender,
            email = user.Email ?? "",
            city = user.city,
            country = user.Country,
            phoneNumber = user.PhoneNumber ?? "",
            emailConfirmed = user.EmailConfirmed
        };

        // Encode user data as base64 to pass safely in URL
        var userDataJson = System.Text.Json.JsonSerializer.Serialize(userData);
        var userDataEncoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(userDataJson));

        // Redirect back to frontend with user data
        var redirectUrl = $"{googleSuccessUrl}?status=success&userData={Uri.EscapeDataString(userDataEncoded)}";

        // Optionally, pass the state parameter back for CSRF protection
        if (!string.IsNullOrEmpty(state))
            redirectUrl += $"&state={Uri.EscapeDataString(state)}";

        return Redirect(redirectUrl);
    }

    [HttpPost("google-signout")]
    public async Task<IActionResult> SignOutUser()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Signed out successfully" });
    }
}
