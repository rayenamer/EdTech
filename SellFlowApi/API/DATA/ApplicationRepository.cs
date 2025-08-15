using System;
using API.Data;
using API.entities;
using Microsoft.EntityFrameworkCore;

namespace API.DATA;

public class ApplicationRepository : IApplicationRepository
{
    private readonly DataContext _context;

    public ApplicationRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Application>> GetAllAsync()
    {
        return await _context.Applications
            .Include(a => a.Documents)
            .ToListAsync();
    }

    public async Task<Application?> GetByIdAsync(int id)
    {
        return await _context.Applications
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Application> AddAsync(Application application)
    {
        _context.Applications.Add(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null)
            return false;

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Document> AddDocumentAsync(int applicationId, Document document)
    {
        var application = await _context.Applications.FindAsync(applicationId);
        if (application == null)
            throw new ArgumentException("Application not found", nameof(applicationId));

        document.ApplicationId = applicationId;
        application.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task<Document?> GetDocumentByIdAsync(int documentId)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == documentId);
    }

    public async Task<IEnumerable<Application>> GetByUserIdAsync(int userId)
    {
        return await _context.Applications
            .Include(a => a.Documents)
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }
}
