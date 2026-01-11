using FileService.Domain.Models;

namespace FileService.Application.DTOs.GroupDTOs;

public interface IFileRepository
{
    Task AddAsync(UserFileMetadata file);
    Task<List<UserFileMetadata>> GetFilesByUserIdAsync(Guid userId);
    Task DeleteAsync(UserFileMetadata file);
    
    Task<UserFileMetadata> GetFileByIdAsync(Guid fileId, Guid userId);
    
}