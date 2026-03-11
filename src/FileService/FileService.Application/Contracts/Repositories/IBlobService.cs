using FileService.Application.DTOs.BlobDTOs;

namespace FileService.Application.Contracts.Repositories;

public interface IBlobService
{
    Task<BlobUploadResult> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    
    Task<bool> DeleteAsync(string blobPath, CancellationToken ct = default);
    
    string GetReadOnlyLink(string blobPath, TimeSpan expiry);
}