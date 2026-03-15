using Microsoft.AspNetCore.Http;

namespace FileService.Application.DTOs.BlobDTOs;

public record UploadFileRequest(
    IFormFile File, 
    string Title
);