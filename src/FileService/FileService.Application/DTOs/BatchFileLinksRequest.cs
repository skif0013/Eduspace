namespace FileService.Application.DTOs;

public record BatchFileLinksRequest(IReadOnlyCollection<Guid> FileIds);