namespace FileService.Application.DTOs.BlobDTOs;

public record DeleteFileCommand(
    Guid FileId,
    Guid UserId );