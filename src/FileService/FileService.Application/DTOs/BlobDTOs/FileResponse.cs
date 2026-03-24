namespace FileService.Application.DTOs.BlobDTOs;

public record FileResponse(
    Guid Id = default,
    string Title = "",
    string Url = "",
    string SizeInBytes = "",
    DateTime CreatedAt = default);
    