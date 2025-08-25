using System;
using API.entities;

namespace API.interfaces;

public interface IDocumentRepository
{
    Task<Document> AddAsync(Document document);
    Task<Document?> GetByIdAsync(int id);
    Task<List<Document>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
    Task<bool> GetDocByNameAndUserDataId(string documentName, int userDataId);
    Task<bool> DeleteDocByNameAndUserDataId(string documentName,int userDataId);
}
