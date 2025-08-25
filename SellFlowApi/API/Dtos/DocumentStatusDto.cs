using System;

namespace API.Dtos;

public class DocumentStatusDto
{
    public bool Cv { get; set; }
    public bool Baccalaureat { get; set; }
    public bool BaccalaureatGrades { get; set; }
    public bool Bachelor { get; set; }
    public bool BachelorGrades { get; set; }
}