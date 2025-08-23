using System;
using API.Data;
using API.entities;
using API.Entities;
using API.interfaces;
using Microsoft.EntityFrameworkCore;
using API.interfaces;
using API.Dtos;
namespace API.DATA;

public class UserDataRepository : IUserDataRepository
{
    private readonly DataContext _context;

    public UserDataRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AppUser>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .ToListAsync(); // No documents loaded
    }

    public async Task<AppUser?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<AppUser> AddAsync(AppUser user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(AppUser user)
    {
        try
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                Console.WriteLine($"User with ID {user.Id} not found for update");
                return false;
            }

            Console.WriteLine($"Updating User ID: {user.Id}");
            Console.WriteLine($"Old values: FullName={existingUser.FullName}, Number={existingUser.Number}");
            Console.WriteLine($"New values: FullName={user.FullName}, Number={user.Number}");

            // Update all the properties
            existingUser.FullName = user.FullName;
            existingUser.Number = user.Number;
            existingUser.UserDataDateOfBirth = user.UserDataDateOfBirth;
            existingUser.Motivation = user.Motivation;
            existingUser.LifeOutSide = user.LifeOutSide;
            existingUser.BaccalaureatDegree = user.BaccalaureatDegree;
            existingUser.BaccalaureatInstitution = user.BaccalaureatInstitution;
            existingUser.BaccalaureatDate = user.BaccalaureatDate;
            existingUser.BachelorDegree = user.BachelorDegree;
            existingUser.BachelorInstitution = user.BachelorInstitution;
            existingUser.BachelorDate = user.BachelorDate;
            existingUser.MasterDegree = user.MasterDegree;
            existingUser.MasterInstitution = user.MasterInstitution;
            existingUser.MasterDate = user.MasterDate;
            existingUser.EngDegree = user.EngDegree;
            existingUser.EngInstitution = user.EngInstitution;
            existingUser.EngDate = user.EngDate;
            existingUser.WorkExperience = user.WorkExperience;
            existingUser.LinkedinLink = user.LinkedinLink;
            existingUser.UserDataExists = user.UserDataExists;

            // Mark as modified
            _context.Users.Update(existingUser);

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
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePersonalinfo(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        user.FullName = string.Empty;
        user.Number = string.Empty;
        user.UserDataDateOfBirth = DateTime.MinValue;
        user.LinkedinLink = null;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }



    public async Task<AppUser?> GetByUserIdAsync(int userId)
    {
        try
        {
            Console.WriteLine($"GetByUserIdAsync called for UserId: {userId}");

            var user = await _context.Users
                .Include(a => a.Documents)
                .FirstOrDefaultAsync(a => a.Id == userId);

            if (user != null)
            {
                Console.WriteLine($"Found user: ID: {user.Id}, FullName: {user.FullName}");
            }
            else
            {
                Console.WriteLine($"No user found with ID: {userId}");
            }

            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetByUserIdAsync: {ex.Message}");
            throw;
        }
    }
    public async Task<bool> AddDocumentAsync(int userId, int documentId)
    {
        var document = await _context.Documents.FindAsync(documentId);
        if (document == null) return false;

        document.UserDataId = userId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AppUser?> FindByEmailAsync(string emailClaim)
    {
        try
        {
            // Recherche des utilisateurs par email
            var user = await _context.Users
                .Include(u => u.Documents)
                .FirstOrDefaultAsync(u => u.Email == emailClaim);

            if (user == null)
            {
                Console.WriteLine($"No user found with email: {emailClaim}");
                return null;
            }

            Console.WriteLine($"Found user with email: {emailClaim}, FullName: {user.FullName}");
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in FindByEmailAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<AppUser?> GetUserWithDocumentsAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new AppUser
                {
                    // Only select needed user fields - exclude sensitive data
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FullName = u.FullName,
                    Number = u.Number,
                    DateOfBirth = u.DateOfBirth,
                    city = u.city, // Required field
                    Gender = u.Gender, // Required field
                    Country = u.Country, // Required field
                    Motivation = u.Motivation,
                    LifeOutSide = u.LifeOutSide,
                    BaccalaureatDegree = u.BaccalaureatDegree,
                    BaccalaureatInstitution = u.BaccalaureatInstitution,
                    BaccalaureatDate = u.BaccalaureatDate,
                    BachelorDegree = u.BachelorDegree,
                    BachelorInstitution = u.BachelorInstitution,
                    BachelorDate = u.BachelorDate,
                    MasterDegree = u.MasterDegree,
                    MasterInstitution = u.MasterInstitution,
                    MasterDate = u.MasterDate,
                    EngDegree = u.EngDegree,
                    EngInstitution = u.EngInstitution,
                    EngDate = u.EngDate,
                    WorkExperience = u.WorkExperience,
                    LinkedinLink = u.LinkedinLink,
                    // Only select minimal document fields - exclude Bytes
                    Documents = u.Documents.Select(d => new Document
                    {
                        Id = d.Id,
                        DocumentName = d.DocumentName,
                        UserDataId = d.UserDataId
                        // Explicitly exclude Bytes field for performance
                    }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserWithDocumentsAsync: {ex.Message}");
            throw;
        }
    }
    public async Task<bool> UpdatePersonalInformationAsync(int userId, PersonalInformationDto personalInfoDto)
    {
        try
        {
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            // Update only the specific fields
            user.FullName = personalInfoDto.FullName;
            user.Number = personalInfoDto.Number;
            user.LinkedinLink = personalInfoDto.LinkedinLink;

            if (personalInfoDto.DateOfBirth.HasValue)
            {
                user.UserDataDateOfBirth = personalInfoDto.DateOfBirth.Value;
            }

            // Update timestamp
            user.LastActive = DateTime.UtcNow;

            var rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
           // _logger.LogError(ex, "Error updating personal information for UserId: {UserId}", userId);
            throw;
        }
    }
}
