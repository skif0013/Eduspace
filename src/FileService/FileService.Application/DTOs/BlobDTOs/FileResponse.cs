namespace FileService.Application.DTOs.BlobDTOs;

public record FileResponse(
    Guid Id,
    string Title,
    string Url,
    string SizeInBytes,
    DateTime CreatedAt)
{
    public FileResponse() : this(Guid.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue) { }
}