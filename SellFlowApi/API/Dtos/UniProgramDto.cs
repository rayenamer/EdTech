using System;

namespace API.Dtos;

public class UniProgramDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime ProgramStart { get; set; } = DateTime.MinValue;

    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public string University { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;

    public int Duration { get; set; } 
}
