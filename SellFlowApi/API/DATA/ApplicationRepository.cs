using System;
using API.Data;
using API.entities;
using Microsoft.EntityFrameworkCore;

namespace API.DATA;

public class ApplicationRepository : IApplicationRepository
{
    private readonly DataContext _context; // 

    public ApplicationRepository(DataContext context, ILogger<ApplicationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    private readonly ILogger<ApplicationRepository> _logger;
    public async Task<Application> CreateApplicationAsync(Application application, int userId, int programId)
    {
        
        application.UserId = userId;
        application.ProgramId = programId;
        application.SubmissionDate = DateTime.UtcNow; // Set submission date

        _context.Applications.Add(application);
        await _context.SaveChangesAsync(); // Actually save to database

        return application;
    }

    public async Task<bool> DeleteApplicationAsync(int applicationId)
    {
        var application = await _context.Applications.FindAsync(applicationId);
        if (application == null) return false;

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Application>> GetAllApplicationsAsync()
    {
        return await _context.Applications.ToListAsync();
    }



    public async Task<IEnumerable<Application>> GetApplicationsByProgramIdAsync(int programId)
    {
        var applications = _context.Applications
            .Where(a => a.ProgramId == programId)
            .ToListAsync();

        return await applications;
    }

    public async Task<IEnumerable<Application>> GetApplicationsByUserIdAsync(int userId)
    {
        var applications = _context.Applications
            .Where(a => a.UserId == userId)
            .ToListAsync();

        return await applications;
    }
    //helper function to get connected user id

}
