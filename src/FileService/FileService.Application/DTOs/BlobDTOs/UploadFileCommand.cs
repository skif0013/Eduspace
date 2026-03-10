namespace FileService.Application.DTOs.BlobDTOs;

public record UploadFileCommand(
    Stream FileStream,
    string FileName,
    string ContentType,
    Guid UserId,
    string Title );