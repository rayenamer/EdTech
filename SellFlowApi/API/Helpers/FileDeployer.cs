using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace API.Helpers
{
    public class FileDeployer
    {
        public FileResult Deploy(byte[] fileBytes, string fileName, string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentNullException(nameof(extension), "Extension cannot be null or empty");
            }

            // Ensure extension starts with a dot
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            // Get content type based on extension
            string contentType = GetContentType(extension);

            // Add extension to filename if not present
            if (!fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                fileName += extension;
            }

            return CreateFileResult(fileBytes, fileName, contentType); // Changed method name
        }

        // Renamed from Deploy to CreateFileResult
        private FileResult CreateFileResult(byte[] fileBytes, string fileName, string contentType)
        {
            // Validate input
            if (fileBytes == null || fileBytes.Length == 0)
            {
                throw new ArgumentException("File bytes cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name cannot be null or empty");
            }

            // Create memory stream from bytes
            var memoryStream = new MemoryStream(fileBytes);

            // Return FileStreamResult for download
            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = fileName
            };
        }

        private string GetContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }
        public string DetectFileExtension(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
                return ".bin";

            // Check file signatures (magic numbers)
            if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                return ".jpg";

            if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                return ".png";

            if (bytes.Length >= 6 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
                return ".gif";

            if (bytes.Length >= 4 && bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46)
                return ".pdf";

            // Default to .bin if can't detect
            return ".bin";
        }
    }
}