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

    public  async Task AddAsync(UserFileMetadata file, CancellationToken ct = default)
    {
        var add = await _dbContext.UserFileMetadatas.AddAsync(file);
    }

    public Task<List<UserFileMetadata>> GetFilesByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return _dbContext.UserFileMetadatas.Where(x => x.UserId == userId).ToListAsync(ct);
    }

    public  async Task<UserFileMetadata?> GetFileByIdAsync(Guid fileId, Guid userId, CancellationToken ct = default)
    {
        return await _dbContext.UserFileMetadatas.FirstOrDefaultAsync(f => f.UserId == userId && f.Id == fileId, ct);
    }

    public void Update(UserFileMetadata file)
    {
        _dbContext.UserFileMetadatas.Update(file);
    }

    public void Remove(UserFileMetadata file)
    {
        _dbContext.UserFileMetadatas.Remove(file);
    }
}