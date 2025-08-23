using System;
using API.Dtos;
using API.entities;
using API.Entities;
namespace API.interfaces;

public interface IUserDataRepository
{
    Task<IEnumerable<AppUser>> GetAllAsync();
    Task<AppUser?> GetByIdAsync(int id);
    Task<AppUser> AddAsync(AppUser user);
    Task<bool> UpdateAsync(AppUser user);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeletePersonalinfo(int id);
    Task<bool> AddDocumentAsync(int userId, int documentId);
    Task<AppUser?> GetByUserIdAsync(int userId);
    Task<AppUser?> FindByEmailAsync(string emailClaim);
    Task<AppUser?> GetUserWithDocumentsAsync(int userId);
    Task<bool> UpdatePersonalInformationAsync(int userId, PersonalInformationDto personalInfoDto);

}
