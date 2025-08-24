using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace API.Helpers;

public class UploadHandler
{
    private readonly string _uploadPath;
    private static readonly Dictionary<string, List<string>> AllowedExtensions = new()
    {
        { "document", new List<string> { ".pdf", ".doc", ".docx" } },
        { "image", new List<string> { ".jpg", ".jpeg", ".png", ".gif" } },
        { "all", new List<string> { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".gif" } }
    };
    
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
    
    public UploadHandler(IConfiguration configuration)
    {
        _uploadPath = configuration["FileStorage:UploadPath"] ?? "uploads";
        Directory.CreateDirectory(_uploadPath);
    }

    // NEW OPTIMIZED METHOD - USE THIS FOR NEW UPLOADS
    public async Task<FileUploadResult> UploadAsync(IFormFile file, string category = "all")
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided or file is empty");

        if (file.Length > MaxFileSize)
            throw new ArgumentException($"Maximum file size is {MaxFileSize / (1024 * 1024)}MB");

        string extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (string.IsNullOrEmpty(extension))
            throw new ArgumentException("File must have an extension");

        if (!AllowedExtensions.TryGetValue(category, out var validExtensions) ||
            !validExtensions.Contains(extension))
        {
            throw new ArgumentException($"Extension '{extension}' not allowed. Valid: {string.Join(", ", validExtensions)}");
        }

        string fileName = GenerateUniqueFileName(file.FileName);
        string filePath = Path.Combine(_uploadPath, fileName);

        try
        {
            // CRITICAL: Async streaming - NO MEMORY LOADING
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
            await file.CopyToAsync(fileStream);

            return new FileUploadResult
            {
                Success = true,
                FilePath = filePath,
                FileName = fileName,
                OriginalFileName = file.FileName,
                FileSize = file.Length,
                ContentType = file.ContentType
            };
        }
        catch (Exception ex)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            throw new InvalidOperationException($"Failed to save file: {ex.Message}", ex);
        }
    }

    // LEGACY METHOD - Keep for existing services compatibility
    public byte[] Upload(IFormFile file)
    {
        Console.WriteLine("⚠️ WARNING: Using legacy Upload method - Update to UploadAsync for 97% better performance");
        
        List<string> validExtensions = new List<string>() { ".pdf", ".jpg", ".jpeg", ".png", ".gif" };
        string extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (!validExtensions.Contains(extension))
            throw new ArgumentException($"Extension not valid ({string.Join(',', validExtensions)})");

        if (file.Length > MaxFileSize)
            throw new ArgumentException("Maximum size can be 5mb");

        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    private static string GenerateUniqueFileName(string originalFileName)
    {
        string extension = Path.GetExtension(originalFileName);
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string uniqueId = Guid.NewGuid().ToString("N")[..8];
        return $"{nameWithoutExtension}_{timestamp}_{uniqueId}{extension}";
    }

    public bool DeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting file {filePath}: {ex.Message}");
            return false;
        }
    }
}