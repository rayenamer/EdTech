using System;

namespace API.Helpers;

public class UploadHandler
{
    public byte[] Upload(IFormFile file)
    {
        // Extension validation
        List<string> validExtensions = new List<string>() { ".jpg", ".png", ".gif" };
        string extension = Path.GetExtension(file.FileName);
        if (!validExtensions.Contains(extension))
        {
            throw new ArgumentException($"Extension is not valid ({string.Join(',', validExtensions)})");
        }

        // File size validation
        long size = file.Length;
        if (size > (5 * 1024 * 1024))
        {
            throw new ArgumentException("Maximum size can be 5mb");
        }

        // Convert file to bytes
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        byte[] fileBytes = memoryStream.ToArray();
        
        return fileBytes;
    }
}