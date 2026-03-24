using FileService.Application.DTOs.BlobDTOs;

namespace FileService.Application.Contracts.Repositories;

public interface IFileService
{
    Task<FileResponse> UploadAsync(UploadFileRequest request, Guid userId, CancellationToken ct = default);
    
    Task<FileResponse> UpdateContentAsync(UpdateFileRequest request, Guid fileId,Guid userId, CancellationToken ct = default);
    
    Task<bool> DeleteAsync(Guid fileId,Guid userId, CancellationToken ct = default);
    
    Task<FileResponse> GetFileLinkAsync(Guid fileId, Guid userId, CancellationToken ct = default);
    
    Task<IEnumerable<FileResponse>> GetAllFilsAsync(Guid userId, CancellationToken ct = default);
    
    Task<Stream> DownloadAsync(Guid fileId, Guid userId, CancellationToken ct = default);
}