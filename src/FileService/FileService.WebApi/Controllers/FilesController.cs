using FileService.Application.Contracts.Repositories;
using FileService.Application.DTOs.BlobDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FileService.WebApi.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;
    
    private readonly Guid _testUserId = Guid.Parse("7a31636c-134b-4654-946d-315147321683");
    
    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FileResponse>> UploadFile([FromForm] UploadFileRequest request, CancellationToken ct)
    {
        var result = await _fileService.UploadAsync(request,_testUserId,ct);
        
        return Ok(result);
    }
    
    [HttpDelete("{fileId}/delete")]
    public async Task<IActionResult> DeleteFile(Guid fileId)
    {
        await _fileService.DeleteAsync(fileId,_testUserId);
        
        return NoContent();
    }

    [HttpPut("{fileId:guid}/update")] 
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FileResponse>> UpdateFile(
        [FromForm] UpdateFileRequest request,
        [FromRoute] Guid fileId, 
        CancellationToken ct)
    {
        var response = await _fileService.UpdateContentAsync(request, fileId, _testUserId, ct);
        
        return Ok(response);
    }

    [HttpGet("{fileId:guid}/link")]
    public async Task<ActionResult<FileResponse>> GetFileLink([FromRoute] Guid fileId, CancellationToken ct)
    {
        var response = await _fileService.GetFileLinkAsync(fileId, _testUserId, ct);

        return Ok(response);
    }

    [HttpGet("GetAllFiles")]
    public async Task<ActionResult<IEnumerable<FileResponse>>> GetAllFilesAsync(CancellationToken ct)
    {
        var response = await _fileService.GetAllFilsAsync(_testUserId, ct);
        
        return Ok(response);
    }
    
    
    [HttpGet("Download/{fileId:guid}")]
    public async Task<IActionResult> DownloadFile([FromRoute]Guid fileId, CancellationToken ct)
    {
        Stream fileStream = await _fileService.DownloadAsync(fileId, _testUserId, ct);
        
        return File(fileStream, "application/octet-stream", "file_from_azure.dat");
    }
}