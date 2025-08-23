using System;
using API.Data;
using API.entities;
using API.interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.DATA
{
    public class UniProgramRepository : IUniProgramRepository
    {
        private readonly DataContext _context;

        public UniProgramRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UniProgram>> GetAllAsync()
        {
            return await _context.UniPrograms
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UniProgram?> GetByIdAsync(int id)
        {
            return await _context.UniPrograms.FindAsync(id);
        }

        public async Task<UniProgram> AddAsync(UniProgram uniProgram)
        {
            _context.UniPrograms.Add(uniProgram);
            await _context.SaveChangesAsync();
            return uniProgram;
        }

        public async Task<UniProgram?> UpdateAsync(UniProgram uniProgram)
        {
            var existing = await _context.UniPrograms.FindAsync(uniProgram.Id);
            if (existing == null)
                return null;

            existing.Name = uniProgram.Name;
            existing.Description = uniProgram.Description;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var uniProgram = await _context.UniPrograms.FindAsync(id);
            if (uniProgram == null)
                return false;

            _context.UniPrograms.Remove(uniProgram);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
