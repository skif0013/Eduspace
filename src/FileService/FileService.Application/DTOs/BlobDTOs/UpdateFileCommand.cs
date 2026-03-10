namespace FileService.Application.DTOs.BlobDTOs;

public record UpdateFileCommand(
    Guid FileId,
    Guid UserId,
    string NewTitle
);