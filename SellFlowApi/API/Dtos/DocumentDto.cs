using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Dtos;

public class DocumentDto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public byte[] ?Bytes { get; set; }
    public string DownloadUrl { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public int UserDataId { get; set; } // Foreign key for UserData
}
