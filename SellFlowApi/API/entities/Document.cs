using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.entities;

public class Document
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    // LEGACY FIELD - Keep for backward compatibility
    public byte[]? Bytes { get; set; }
    
    // NEW OPTIMIZED FIELDS - Filesystem storage
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public string? OriginalFileName { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public DateTime? UploadDate { get; set; }
    
    // STORAGE MODE - Indicates how file is stored
    public string StorageMode { get; set; } = "Database"; // "Database" or "FileSystem"
    
    public int UserDataId { get; set; } // Foreign key for UserData
    public string DocumentName { get; set; } = string.Empty;
}
