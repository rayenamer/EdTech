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
    [HttpPost("register")]
    public async Task<ActionResult<AppUserDto>> Register(RegisterDto registerDto)
    {
        var user = mapper.Map<AppUser>(registerDto);

        var userExist = await userManager.FindByEmailAsync(registerDto.Email);
        if (userExist != null) return BadRequest("Email already Signed");
        //
        user.EmailConfirmed = !registerDto.requiredEmailConfirmation;
        //
        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        // INIT UserData
        await _UserDataRepository.AddAsync(new UserData
        {
            UserId = user.Id, // Connect to the created user
            exists = "true",
            // Add other required properties based on your UserData model
        });

        //
        var emailToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = Url.Action(
        "ConfirmEmail",
        "Register_Login",
        new { userId = user.Id, token = emailToken },
        Request.Scheme
        );


        _ = Task.Run(async () =>
        {
            try
            {
                await _emailSender.SendEmailAsync(
                    registerDto.Email,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email");
            }

            return new AppUserDto
            {
                Username = user.UserName ?? "",
                Token = await tokenService.CreateToken(user),
                Gender = registerDto.Gender,
                city = registerDto.city,
                Country = registerDto.Country,
                PhoneNumber = user.PhoneNumber ?? "",
                Email = user.Email ?? "",
                EmailConfirmed = true // Indicate email needs confirmation
            };

        });
        return Ok(new { Message = "Registration successful. Please check your email." });
    }
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return BadRequest("Invalid request");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("User not found");

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded) return BadRequest("Confirmation failed");

        return Ok("Email confirmed successfully!");
    }

    [HttpPost("login")]
    public async Task<ActionResult<AppUserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(x =>
                x.NormalizedEmail == loginDto.Email.ToUpper());
        _logger.LogInformation($"User: {user?.UserName}, EmailConfirmed: {user?.EmailConfirmed}");
        if (user == null || user.Email == null)
        {
            return Unauthorized("Invalid mail");
        }

        // ⚠️⚠️⚠️ we will comment this so everyone can test on their machines⚠️⚠️⚠️

        //if (!user.EmailConfirmed)
        //{
        //    // If the email is not confirmed, reject login attempt
        //    return Unauthorized("Email is not confirmed.");
        //}




        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result) return Unauthorized();

        return new AppUserDto
        {
            Username = user.UserName ?? "",
            Token = await tokenService.CreateToken(user),
            Gender = user.Gender,
            Email = user.Email,
            city = user.city,
            Country = user.Country,
            PhoneNumber = user.PhoneNumber ?? "",
        };
    }
    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(RequestForgotPasswordDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid payload");

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest("Something went wrong");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        if (string.IsNullOrEmpty(token))
            return BadRequest("No token generated");

        var callbackUrl = $"http://localhost:4200/resetpass?code={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

        // Send email in background
        _ = Task.Run(async () =>
        {
            try
            {
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Reset your password",
                    $"You can reset your password by <a href='{callbackUrl}'>clicking here</a>."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reset password email");
            }
        });

        return Ok(new
        {
            message = "Password reset email sent.",
            email = user.Email
        });
    }


    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest("invalid model or whatever");

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest("no user");

        var result = await userManager.ResetPasswordAsync(user, request.token, request.Password);
        if (result.Succeeded)
            return Ok(new { message = "Password reset is successful" });

        return BadRequest("something went wrong ");
    }

    //
    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
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
                await _UserDataRepository.AddAsync(new UserData
                {
                    UserId = user.Id, // Connect to the created user
                    exists = "true",
                    // Add other required properties based on your UserData model
                });
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

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<AppUserDto>> GetCurrentUser()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return NotFound();
        return new AppUserDto
        {
            Username = user.UserName ?? "",
            Email = user.Email ?? "",
            Gender = user.Gender,
            city = user.city,
            Country = user.Country,
            PhoneNumber = user.PhoneNumber ?? "",
            EmailConfirmed = user.EmailConfirmed,
            Token = await tokenService.CreateToken(user)
        };
    }
}
