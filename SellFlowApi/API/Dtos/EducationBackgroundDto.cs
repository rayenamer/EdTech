using System;

namespace API.Dtos;

public class EducationBackgroundDto
{
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
}
