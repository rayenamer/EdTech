using System;
using API.entities;
namespace API.DATA;

public interface IApplicationRepository
{
    Task<IEnumerable<Application>> GetAllAsync();
    Task<Application?> GetByIdAsync(int id);
    Task<Application> AddAsync(Application application);
    Task<bool> DeleteAsync(int id);
    Task<Document> AddDocumentAsync(int applicationId, Document document);
    Task<Document?> GetDocumentByIdAsync(int documentId);
    Task<IEnumerable<Application>> GetByUserIdAsync(int userId);
}
