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
    Task<bool> AddDocumentAsync(int userDataId, int documentId);
    Task<IEnumerable<UserData>> GetByUserIdAsync(int userId);
}
