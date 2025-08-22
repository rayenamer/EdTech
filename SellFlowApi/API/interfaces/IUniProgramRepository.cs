using System;
using API.entities;

namespace API.interfaces;

public interface IUniProgramRepository
{
    Task<IEnumerable<UniProgram>> GetAllAsync();
    Task<UniProgram?> GetByIdAsync(int id);
    Task<UniProgram> AddAsync(UniProgram uniProgram);
    Task<UniProgram?> UpdateAsync(UniProgram uniProgram);
    Task<bool> DeleteAsync(int id);
}
