using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.entities;

public class Document
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public byte[]? Bytes { get; set; }
    public int UserDataId { get; set; } // Foreign key for UserData
    public string DocumentName { get; set; } = string.Empty;
}
