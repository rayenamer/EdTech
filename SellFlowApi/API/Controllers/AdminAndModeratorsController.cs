using System;
using System.Security.Claims;
using API.Dtos;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminAndModeratorsController
(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IMapper mapper
    //IEmailSender _emailSender,
    //ILogger<AdminAndModeratorsController> _logger
) : BaseApiController
{
    [HttpPost("register-admin")]
    public async Task<ActionResult<AppUserDto>> RegisterAdmin(AdminAndModeratorDto AdminAndModeratorDto)
    {
        var userExist = await userManager.FindByEmailAsync(AdminAndModeratorDto.Email);
        if (userExist != null) return BadRequest("Email already signed");


        var user = mapper.Map<AppUser>(AdminAndModeratorDto);

        var result = await userManager.CreateAsync(user, AdminAndModeratorDto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        await userManager.AddToRoleAsync(user, "Admin");
        var token = await tokenService.CreateToken(user);

        return new AppUserDto
        {
            Username = user.UserName ?? "",
            Token = await tokenService.CreateToken(user),
            Gender = AdminAndModeratorDto.Gender,
            city = AdminAndModeratorDto.city,
            Country = AdminAndModeratorDto.Country,
            PhoneNumber = user.PhoneNumber ?? "",
            Email = user.Email ?? "",
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<AppUserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.NormalizedEmail == loginDto.Email.ToUpper());
        if (user == null || user.Email == null)
        {
            return Unauthorized("invalid email ");
        }

        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result) return Unauthorized("wrong password");

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
    public async Task<ActionResult<AppUserDto>> GetCurrentAdmin()
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
        return Ok(new { message = "Admin logged out successfully" });
    }
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("GetAllUsersForAdmin")]
    public async Task<IActionResult> GetAllUsersForAdmin()
    {


        var users = await userManager.Users
     .OrderBy(x => x.UserName)
     .Select(x => new
     {
         x.Id,
         Username = x.UserName,
         Gender = x.Gender,
         DateOfBirth = x.DateOfBirth,
         Roles = x.UserRoles.Select(r => r.Role.Name).ToList(),
         Email = x.Email,
         City = x.city,
         LastActive = x.LastActive.ToString("f"),
     })
     .ToListAsync();
        return Ok(users);

    }

    [Authorize]
    [HttpGet("secure")]
    public IActionResult Secure()
    {
        return Ok("This endpoint is secure");
    }

    [HttpGet("token-info")]
    public IActionResult TokenInfo()
    {
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
        var claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();
        
        return Ok(new { isAuthenticated, claims });
    }


}
