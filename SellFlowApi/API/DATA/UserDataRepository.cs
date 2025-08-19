using System;
using API.Data;
using API.entities;
using Microsoft.AspNetCore.Http.HttpResults;
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

            // Update all the properties
            existingUserData.FullName = userData.FullName;
            existingUserData.Number = userData.Number;
            existingUserData.DateOfBirth = userData.DateOfBirth;
            existingUserData.Motivation = userData.Motivation;
            existingUserData.LifeOutSide = userData.LifeOutSide;
            existingUserData.BaccalaureatDegree = userData.BaccalaureatDegree;
            existingUserData.BaccalaureatInstitution = userData.BaccalaureatInstitution;
            existingUserData.BaccalaureatDate = userData.BaccalaureatDate;
            existingUserData.BachelorDegree = userData.BachelorDegree;
            existingUserData.BachelorInstitution = userData.BachelorInstitution;
            existingUserData.BachelorDate = userData.BachelorDate;
            existingUserData.MasterDegree = userData.MasterDegree;
            existingUserData.MasterInstitution = userData.MasterInstitution;
            existingUserData.MasterDate = userData.MasterDate;
            existingUserData.EngDegree = userData.EngDegree;
            existingUserData.EngInstitution = userData.EngInstitution;
            existingUserData.EngDate = userData.EngDate;
            existingUserData.WorkExperience = userData.WorkExperience;
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
     public async Task<bool> AddDocumentAsync(int userDataId, int documentId)
    {
        var document = await _context.Documents.FindAsync(documentId);
        if (document == null) return false;

        document.UserDataId = userDataId;
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task FindByEmailAsync(string emailClaim)
    {
        try
        {
            // Recherche des utilisateurs par email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim);
            if (user == null)
            {
                Console.WriteLine($"No user found with email: {emailClaim}");
                return;
            }

            // Récupération des données utilisateur associées
            var userDatas = await _context.UserDatas
                .Where(ud => ud.UserId == user.Id)
                .ToListAsync();

            Console.WriteLine($"Found {userDatas.Count} UserData records for user with email: {emailClaim}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in FindByEmailAsync: {ex.Message}");
            throw;
        }
    }
}
