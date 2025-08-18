using System;
using System.ComponentModel.DataAnnotations;
using API.entities;

namespace API.Dtos;

public class UserDataDto
{
    public int Id { get; set; }
    
    [Required]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    public string Number { get; set; } = string.Empty;
    
    [Required]
    public DateTime DateOfBirth { get; set; }
    
    [Required]
    public string Motivation { get; set; } = string.Empty;
    
    [Required]
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
    
    public int UserId { get; set; } 



    public List<DocumentDto>? Documents { get; set; } = new List<DocumentDto>();
}
