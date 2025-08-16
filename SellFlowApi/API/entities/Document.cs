using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.entities;

public class Document
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>(); // the document content is stored as a byte array

    public Document()
    {
        UploadDate = DateTime.UtcNow;
    }
    public int UserDataId { get; set; }
    public UserData? UserData { get; set; } // Navigation property to UserData
    public string DocumentType { get; set; } = string.Empty;



    // migrating to cloud
    //public int Id{get;set;}
    //public required string Url{get;set;}
    //public string? PublicId{get;set;}
}
