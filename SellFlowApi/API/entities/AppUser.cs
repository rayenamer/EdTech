using System;
using EllipticCurve.Utils;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser<int>
{
        public DateOnly DateOfBirth {get; set;}
        public DateTime Created{get;set;}=DateTime.UtcNow;
        public DateTime LastActive{get;set;}=DateTime.UtcNow;
        public required string Gender{get;set;}
        
        public required string city{get;set;}
        public required string Country{get;set;}
        public ICollection<AppUserRole> UserRoles { get; set; } = [];

}