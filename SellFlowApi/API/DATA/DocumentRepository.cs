using System;
using API.Data;
using API.entities;
using Microsoft.EntityFrameworkCore;
using API.interfaces;

namespace API.DATA;

public class DocumentRepository : IDocumentRepository
{
    private readonly DataContext _context; // Replace with your DbContext name

    public DocumentRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Document> AddAsync(Document document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task<Document?> GetByIdAsync(int id)
    {
        return await _context.Documents.FindAsync(id);
    }

    public async Task<List<Document>> GetAllAsync()
    {
        return await _context.Documents.ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document == null) return false;

        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> GetDocByNameAndUserDataId(string documentName, int userDataId)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentName == documentName && d.UserDataId == userDataId);
        return document != null;
    }

    public async Task<bool> DeleteDocByNameAndUserDataId(string documentName,int userDataId)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.DocumentName == documentName && d.UserDataId == userDataId);
        if (document == null) return false;

        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
        return true;
    }
}
