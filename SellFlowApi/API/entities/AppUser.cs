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

    // UserData attributes moved directly to AppUser
    public string FullName { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public DateTime UserDataDateOfBirth { get; set; }
    public string Motivation { get; set; } = string.Empty;
    public string LifeOutSide { get; set; } = string.Empty;
    public string? BaccalaureatDegree { get; set; }
    public string? BaccalaureatInstitution { get; set; }
    public DateTime? BaccalaureatDate { get; set; }
    public string? BachelorDegree { get; set; }
    public string? BachelorInstitution { get; set; }
    public DateTime? BachelorDate { get; set; }
    public string? MasterDegree { get; set; }
    public string? MasterInstitution { get; set; }
    public DateTime? MasterDate { get; set; }
    public string? EngDegree { get; set; }
    public string? EngInstitution { get; set; }
    public DateTime? EngDate { get; set; }
    public string? WorkExperience { get; set; }
    public string? LinkedinLink { get; set; }
    public string UserDataExists { get; set; } = string.Empty; // Indicates if the user data exists

    public List<Document>? Documents { get; set; } = new List<Document>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}