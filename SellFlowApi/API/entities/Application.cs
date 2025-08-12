using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities;

namespace API.entities;

public class Application
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Number { get; set;}
    public string Status { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Motivation{ get; set; }
    public string LifeOutSide { get; set; }
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
    public int UserId { get; set; } // Foreign key for AppUser
    public AppUser User { get; set; } // Navigation property to AppUser
    public int ProgramId { get; set; }
    public UniProgram Program { get; set; } // Navigation property to UniProgram
    public List<Document> Documents { get; set; } = new List<Document>();

    public Application()
    {
        CreatedAt = DateTime.UtcNow;
        Status = "Pending"; // Default status
    }
}
