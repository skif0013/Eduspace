using FileService.Application.DTOs.BlobDTOs;

namespace FileService.Application.DTOs;

public record BatchFileLinksResponse(IReadOnlyCollection<FileResponse> Files, IReadOnlyCollection<Guid> NotFoundFileIds);