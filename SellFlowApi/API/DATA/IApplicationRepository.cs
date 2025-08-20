using System;
using API.entities;

namespace API.DATA;

public interface IApplicationRepository
{
    Task<IEnumerable<Application>> GetApplicationsByUserIdAsync(int userId);
    Task<IEnumerable<Application>> GetApplicationsByProgramIdAsync(int programId);
    Task<IEnumerable<Application>> GetAllApplicationsAsync();
    Task<Application> CreateApplicationAsync(Application application, int userId, int programId);
    Task<bool> DeleteApplicationAsync(int applicationId);
}
