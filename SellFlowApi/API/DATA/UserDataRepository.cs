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

    public async Task<bool> DeleteAsync(int id)
    {
        var userData = await _context.UserDatas.FindAsync(id);
        if (userData == null)
            return false;

        _context.UserDatas.Remove(userData);
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
        return await _context.UserDatas
            .Include(a => a.Documents)
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }
}
