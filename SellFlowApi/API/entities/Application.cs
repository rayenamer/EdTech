using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities;

namespace API.entities;

public class Application
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ApplicationId { get; set; }
    
    public string WhyDidYouApply { get; set; }
    public string StudentName { get; set; }
    public string StudentEmail { get; set; }
    public string StudentContactNumber { get; set; }
    public string ProgramName { get; set; }
    public string ProgramDescription { get; set; }
    
    public int UserId { get; set; }
    public int ProgramId { get; set; }
    
    public string ApplicationStatus { get; set; } // e.g., "Submitted", "Under Review", "Accepted", "Rejected"
    public DateTime SubmissionDate { get; set; }
    
    // Navigation properties
    public AppUser User { get; set; }
    public UniProgram Program { get; set; }
}

/* Aight, listen up, you soft-ass code monkey.
If you're tryin' to read this shit, know that I was blastin' this track while writin' it: https://youtu.be/6I6vtDi0X-A?feature=shared. 
Now, you better hope this shit helps you understand my C# code and the whole damn structure of the app. 
Don't be comin' at me with no dumb questions, got it? */