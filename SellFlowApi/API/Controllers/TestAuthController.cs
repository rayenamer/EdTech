using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestAuthController : ControllerBase
{
    private readonly ILogger<TestAuthController> _logger;

    public TestAuthController(ILogger<TestAuthController> logger)
    {
        _logger = logger;
    }

    [HttpGet("open")]
    public IActionResult Open()
    {
        _logger.LogInformation("Open endpoint called");
        return Ok("Open endpoint working");
    }

    [Authorize]
    [HttpGet("secure")]
    public IActionResult Secure()
    {
        _logger.LogInformation("Secure endpoint called");
        var user = User;
        return Ok(new { 
            Message = "Secure endpoint working",
            Username = User.Identity?.Name,
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        _logger.LogInformation("Admin endpoint called");
        return Ok("Admin endpoint working");
    }

    [Authorize]
    [HttpGet("debug-role")]
    public IActionResult DebugRole()
    {
        var isAdmin = User.IsInRole("Admin");
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
        
        return Ok(new {
            IsAdmin = isAdmin,
            Roles = roles,
            AllClaims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
} 