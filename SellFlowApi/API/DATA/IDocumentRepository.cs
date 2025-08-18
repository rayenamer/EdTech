using System;
using API.entities;

namespace API.DATA;

public interface IDocumentRepository
{
    Task<Document> AddAsync(Document document);
    Task<Document?> GetByIdAsync(int id);
    Task<List<Document>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
    Task<bool> GetDocByName(string documentName);
    Task<bool> DeleteDocByName(string documentName);
}
