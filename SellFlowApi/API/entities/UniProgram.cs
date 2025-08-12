using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.entities;

public class UniProgram
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ProgramStart { get; set; } = DateTime.MinValue;

    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public string University { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;

    public int Duration { get; set; }
    public ICollection<Application> Applications { get; set; } = [];
    
}
