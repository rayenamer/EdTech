using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Controllers;

public class DevTools
(
    UserManager<AppUser> userManager,
    Log log
) : BaseApiController
{
    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsersForAdmin()
    {
        log.LogInformation("🚀 DevTools: Getting all users");
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

    [HttpGet("DeleletAllUser")]
    public async Task<ActionResult> DeleteAllUsers()
    {
        log.LogInformation("🚀 DevTools: Deleting all users");
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
