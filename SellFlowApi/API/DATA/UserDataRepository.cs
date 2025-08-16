using System;
using API.Data;
using API.entities;
using Microsoft.EntityFrameworkCore;

namespace API.DATA;

public class UserDataRepository : IUserDataRepository
{
    private readonly DataContext _context;

    public UserDataRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserData>> GetAllAsync()
    {
        return await _context.UserDatas
            .Include(a => a.Documents)
            .ToListAsync();
    }

    public async Task<UserData?> GetByIdAsync(int id)
    {
        return await _context.UserDatas
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<UserData> AddAsync(UserData userData)
    {
        _context.UserDatas.Add(userData);
        await _context.SaveChangesAsync();
        return userData;
    }

    public async Task<bool> UpdateAsync(UserData userData)
    {
        try
        {
            var existingUserData = await _context.UserDatas.FindAsync(userData.Id);
            if (existingUserData == null)
            {
                Console.WriteLine($"UserData with ID {userData.Id} not found for update");
                return false;
            }

            Console.WriteLine($"Updating UserData ID: {userData.Id}");
            Console.WriteLine($"Old values: FullName={existingUserData.FullName}, Number={existingUserData.Number}");
            Console.WriteLine($"New values: FullName={userData.FullName}, Number={userData.Number}");

            // Update the properties
            existingUserData.FullName = userData.FullName;
            existingUserData.Number = userData.Number;
            existingUserData.DateOfBirth = userData.DateOfBirth;
            existingUserData.LinkedinLink = userData.LinkedinLink;

            // Mark as modified
            _context.UserDatas.Update(existingUserData);
            
            var result = await _context.SaveChangesAsync();
            Console.WriteLine($"SaveChanges result: {result} rows affected");
            
            return result > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var userData = await _context.UserDatas.FindAsync(id);
        if (userData == null)
            return false;

        _context.UserDatas.Remove(userData);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeletePersonalinfo(int id)
    {
        var userData = await _context.UserDatas.FindAsync(id);
        if (userData == null)
            return false;

        userData.FullName = string.Empty;
        userData.Number = string.Empty;
        userData.DateOfBirth = DateTime.MinValue;
        userData.LinkedinLink = null;

        _context.UserDatas.Update(userData);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Document> AddDocumentAsync(int userDataId, Document document)
    {
        var userData = await _context.UserDatas.FindAsync(userDataId);
        if (userData == null)
            throw new ArgumentException("UserData not found", nameof(userDataId));

        document.UserDataId = userDataId;
        userData.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task<Document?> GetDocumentByIdAsync(int documentId)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == documentId);
    }

    public async Task<IEnumerable<UserData>> GetByUserIdAsync(int userId)
    {
        try
        {
            Console.WriteLine($"GetByUserIdAsync called for UserId: {userId}");
            
            var userDatas = await _context.UserDatas
                .Include(a => a.Documents)
                .Where(a => a.UserId == userId)
                .ToListAsync();
            
            Console.WriteLine($"Found {userDatas.Count} UserData records for UserId: {userId}");
            foreach (var ud in userDatas)
            {
                Console.WriteLine($"UserData ID: {ud.Id}, FullName: {ud.FullName}, UserId: {ud.UserId}");
            }
            
            return userDatas;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetByUserIdAsync: {ex.Message}");
            throw;
        }
    }
}
