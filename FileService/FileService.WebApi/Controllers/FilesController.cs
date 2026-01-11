using FileService.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.WebApi.Controllers;

[ApiController]
public class FilesController : ControllerBase
{
    private readonly BlobStorageService _blobService;


    public FilesController(BlobStorageService blobService)
    {
        _blobService = blobService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFile(IFormFile file, Guid userId)
    {
        using var stream = file.OpenReadStream();

        var result = await _blobService.UploadFileAsync(stream, file.FileName, userId);

        return Ok(result);
    }


    [HttpGet("{fileId}/link")]
    public async Task<IActionResult> GetFileLink(Guid fileId, Guid userId)
    {
        var url = await _blobService.GetFileLinkAsync(fileId, userId);

        return Ok(url);
    }

    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile(Guid fileId, Guid userId)
    {
        await _blobService.DeleteFileAsync(fileId, userId);

        return NoContent();
    }
}
    