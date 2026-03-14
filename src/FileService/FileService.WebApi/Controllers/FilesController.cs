using System.Security.Claims;
using AutoMapper;
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
   
    
    
    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FileResponse>> UploadFile([FromForm] UploadFileRequest request, CancellationToken ct)
    {
        Guid fakeUserId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        
        var result = await _fileService.UploadAsync(request,fakeUserId,ct);
        
        return Ok(result);
    }
    
    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile(Guid fileId)
    {
        
        Guid fakeUserId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        
        await _fileService.DeleteAsync(fileId,fakeUserId);
        
        return NoContent();
    }

    [HttpPut("{fileId:guid}/update")] 
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FileResponse>> UpdateFile(
        [FromForm] UpdateFileRequest request,
        [FromRoute] Guid fileId, 
        CancellationToken ct)
    {
        Guid fakeUserId = Guid.Parse("00000000-0000-0000-0000-000000000000");
        
        
        var response = await _fileService.UpdateContentAsync(request, fileId, fakeUserId, ct);
    
        // 3. Возвращаем именно то, что пришло из сервиса
        return Ok(response);
    }
}