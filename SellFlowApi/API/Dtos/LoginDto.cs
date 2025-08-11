using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace API.Dtos;

public class LoginDto
{
    public required string Email{get;set;}
    public required string Password {get; set;}
}
