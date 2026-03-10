namespace FileService.Application.DTOs.BlobDTOs;

public record FileResponse(
    Guid Id,
    string Title,
    string Url,
    string SizeInBytes,
    DateTime CreatedAt );