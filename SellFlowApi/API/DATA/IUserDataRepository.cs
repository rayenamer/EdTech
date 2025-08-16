using System;
using API.entities;
namespace API.DATA;

public interface IUserDataRepository
{
    Task<IEnumerable<UserData>> GetAllAsync();
    Task<UserData?> GetByIdAsync(int id);
    Task<UserData> AddAsync(UserData userData);
    Task<bool> UpdateAsync(UserData userData);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeletePersonalinfo(int id);
    Task<Document> AddDocumentAsync(int userDataId, Document document);
    Task<Document?> GetDocumentByIdAsync(int documentId);
    Task<IEnumerable<UserData>> GetByUserIdAsync(int userId);
}
