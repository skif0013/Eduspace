using FileService.Domain.Models;

namespace FileService.Application.Contracts.Repositories;

public interface IFileRepository
{
    Task AddAsync(UserFileMetadata file);
    Task<List<UserFileMetadata>> GetFilesByUserIdAsync(Guid userId);
    Task DeleteAsync(UserFileMetadata file);
    
    Task<UserFileMetadata> GetFileByIdAsync(Guid fileId, Guid userId);
    
}