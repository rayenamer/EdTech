using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using API.entities;
using API.Helpers;

namespace API.interfaces;

public interface IDocumentService
{
    // NEW OPTIMIZED METHODS - Filesystem storage
    Task<Document> UploadDocumentAsync(IFormFile file, string documentName, int userDataId);
    Task<byte[]?> GetDocumentBytesAsync(int documentId);
    Task<string?> GetDocumentDownloadUrlAsync(int documentId);
    Task<bool> DeleteDocumentAsync(int documentId);
    
    // LEGACY SUPPORT - Database storage
    Task<Document> UploadDocumentLegacyAsync(IFormFile file, string documentName, int userDataId);
    
    // UTILITY METHODS
    Task<bool> DocumentExistsAsync(string documentName, int userDataId);
    Task<bool> DeleteDocumentByNameAsync(string documentName, int userDataId);
}