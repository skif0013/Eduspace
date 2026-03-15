using Microsoft.AspNetCore.Http;

namespace FileService.Application.DTOs.BlobDTOs;

public record UpdateFileRequest(string FileName,IFormFile File);