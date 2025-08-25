using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Dtos;

public class DocumentDto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    // REMOVED: Bytes property eliminated - no binary data in DTOs
    public string DownloadUrl { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public int UserDataId { get; set; } // Foreign key for UserData
}
