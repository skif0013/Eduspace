namespace FileService.Application.DTOs.BlobDTOs;

public record BlobUploadResult(
    string BlobPatch,
    string SaasUrl
    );