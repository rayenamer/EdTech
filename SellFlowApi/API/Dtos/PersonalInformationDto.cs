using System;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class PersonalInformationDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    public string Number { get; set; } = string.Empty;
    
    public DateTime? DateOfBirth { get; set; }
    
    public string? LinkedinLink { get; set; }
}
