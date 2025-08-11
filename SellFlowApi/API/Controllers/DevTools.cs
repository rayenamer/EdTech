using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class DevTools
(
    UserManager<AppUser> userManager
) : BaseApiController
{
    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            // Fetch all users from UserManager
            var users = userManager.Users.ToList();  // This fetches all users

           

            // Return a simplified response with only UserName and EmailConfirmed
            var result = users.Select(u => new { u.UserName, u.EmailConfirmed }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("DeleletAllUser")]
    public async Task<ActionResult> DeleteAllUsers()
    {
        try
        {
            var users = userManager.Users.ExecuteDelete();

            return Ok("all users deleted");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "internal server error ");
        }
    }

}
