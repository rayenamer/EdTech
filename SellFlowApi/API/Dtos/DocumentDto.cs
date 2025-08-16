using System;

namespace API.Dtos;

public class DocumentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public int UserDataId { get; set; }
}
