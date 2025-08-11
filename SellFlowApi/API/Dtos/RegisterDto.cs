using System;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class RegisterDto
{
    public required string Username { get; set; }

    public required string Gender{get;set;}
    public required string city{get;set;}
    public required string Country{get;set;}
    public required string PhoneNumber{get;set;}
    public required string Email{get;set;}

    public string Password {get; set;} = string.Empty;
    public bool requiredEmailConfirmation {get;set;} = true;
}
