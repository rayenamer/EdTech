using System;
using API.Dtos;
using EllipticCurve.Utils;
using Microsoft.AspNetCore.Identity;
using API.Entities;
using API.entities; // Add this if Application is in the same namespace, or replace with the correct namespace

namespace API.Entities;

public class AppUser : IdentityUser<int>
{
    public DateOnly DateOfBirth { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public required string Gender { get; set; }

    public required string city { get; set; }
    public required string Country { get; set; }
    public ICollection<AppUserRole> UserRoles { get; set; } = [];

    public UserData? UserData { get; set; }
    public ICollection<Application> Applications { get; set; } = new List<Application>();

}