using System.Security.Claims;
using FileService.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace FileService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly BlobStorageService _blobService;
    
    
    public FilesController(BlobStorageService blobService)
    {
        _blobService = blobService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var userIdFromClaims = User.FindFirst("userId")?.Value;
        Guid userId = Guid.Parse(userIdFromClaims);
        
        using var stream = file.OpenReadStream();

        var result = await _blobService.UploadFileAsync(stream, file.FileName, userId);

        return Ok(result);
    }
    
    [HttpGet("{fileId}/link")]
    public async Task<IActionResult> GetFileLink(Guid fileId)
    {
        var userIdFromClaims = User.FindFirst("userId")?.Value;
        Guid userId = Guid.Parse(userIdFromClaims);
        
        var url = await _blobService.GetFileLinkAsync(fileId, userId);
        
        return Ok(url);
    }
    
    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile(Guid fileId)
    {
        var userIdFromClaims = User.FindFirst("userId")?.Value;
        Guid userId = Guid.Parse(userIdFromClaims);
        
        await _blobService.DeleteFileAsync(fileId, userId);
        
        return NoContent();
    }
}