using FileService.Domain.Models;

namespace FileService.Application.Contracts.Repositories;

public interface IFileRepository
{
    Task AddAsync(UserFileMetadata file, CancellationToken ct = default);
    
    Task<List<UserFileMetadata>> GetFilesByUserIdAsync(Guid userId, CancellationToken ct = default);
    
    Task<UserFileMetadata?> GetFileByIdAsync(Guid fileId, Guid userId, CancellationToken ct = default);
    
    void Update(UserFileMetadata file);
    
    void Remove(UserFileMetadata file);
    
}