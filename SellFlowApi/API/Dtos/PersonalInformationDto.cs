using System;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class PersonalInformationDto
{

    public string FullName { get; set; } = string.Empty;
    

    public string Number { get; set; } = string.Empty;
    
    public DateTime? DateOfBirth { get; set; }
    
    public string? LinkedinLink { get; set; }
}
