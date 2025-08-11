using System;

namespace API.Dtos;

public class AppUserDto
{
    public required string Username { get; set; }
    public required string Gender { get; set; }
    public required string city { get; set; }
    public required string Country { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public required string Token {get; set;}
    public bool EmailConfirmed { get;  set; }
}
