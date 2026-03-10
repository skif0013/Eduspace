using FileService.Application.Contracts.Repositories;
using FileService.Domain.Models;
using FileService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public FileRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task AddAsync(UserFileMetadata userFile)
    {
       await _dbContext.UserFileMetadatas.AddAsync(userFile);
    }
    
    public async Task<List<UserFileMetadata>> GetFilesByUserIdAsync(Guid userId)
    {
        return await _dbContext.UserFileMetadatas
            .Where(f => f.UserId == userId && !f.IsDeleted)
            .ToListAsync();
    }
    
    public async Task DeleteAsync(UserFileMetadata userFile)
    {
       userFile.IsDeleted = true;
    }
    
    public async Task<UserFileMetadata?> GetFileByIdAsync(Guid fileId, Guid userId)
    {
        return await _dbContext.UserFileMetadatas
            .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);
    }
}