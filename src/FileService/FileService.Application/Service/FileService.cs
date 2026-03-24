using AutoMapper;
using FileService.Application.Contracts.Repositories;
using FileService.Application.DTOs.BlobDTOs;
using FileService.Domain.Models;

namespace FileService.Application.Service;

public class FileService : IFileService
{
    private readonly IBlobService _blobService;
    private readonly IFileRepository _fileRepository;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    
    public FileService(IBlobService blobService, IFileRepository fileRepository, IUnitOfWork uow,  IMapper mapper)
    {
        _mapper = mapper;
        _blobService = blobService;
        _fileRepository = fileRepository;
        _uow = uow;
    }
    
    public async Task<FileResponse> UploadAsync(UploadFileRequest request,Guid userId, CancellationToken ct = default)
    {
        await using var stream = request.File.OpenReadStream();
        
        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
        
        var uploadResult = await _blobService.UploadAsync(stream, uniqueFileName, request.File.ContentType, ct);
        
        var fileMetadata = new UserFileMetadata(
            userId,
            uploadResult.BlobPatch, 
            request.File.Length, 
            request.File.ContentType, 
            request.Title);
        
        await  _fileRepository.AddAsync(fileMetadata, ct);
        await _uow.SaveChangesAsync();


        return _mapper.Map<FileResponse>(fileMetadata) with 
        { 
          Url = uploadResult.SaasUrl 
        };
    }

    public async Task<FileResponse> UpdateContentAsync(UpdateFileRequest request, Guid fileId, Guid userId,CancellationToken ct = default)
    {
        var fileMetadata = await _fileRepository.GetFileByIdAsync(fileId, userId, ct);
        if (fileMetadata == null) 
            throw new Exception("File not found or access denied");
        
        var oldBlobPath = fileMetadata.BlobPath;
        
        using var stream = request.File.OpenReadStream();
        
        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
    
        var uploadResult = await _blobService.UploadAsync(
            stream, 
            uniqueFileName, 
            request.File.ContentType, 
            ct);
        
        fileMetadata.ReplaceContent(
            uploadResult.BlobPatch, 
            request.File.Length, 
            request.File.ContentType);
        
        _fileRepository.Update(fileMetadata);
        
        await _uow.SaveChangesAsync();

        await _blobService.DeleteAsync(oldBlobPath, ct);
        
        return _mapper.Map<FileResponse>(fileMetadata) with 
        { 
            Url = uploadResult.SaasUrl 
        };
    }

    public async Task<bool> DeleteAsync(Guid fileId,Guid userId, CancellationToken ct = default)
    {
        var fileMetadata = await  _fileRepository.GetFileByIdAsync(fileId, userId, ct);

        if (fileMetadata == null)
        {
            throw new Exception("File not found or access denied");
        }

        await _blobService.DeleteAsync(fileMetadata.BlobPath, ct);
        
        fileMetadata.Delete();
        
        _fileRepository.Remove(fileMetadata);
        await _uow.SaveChangesAsync();

        return true;
    }

    public async Task<FileResponse> GetFileLinkAsync(Guid fileId, Guid userId, CancellationToken ct = default)
    {
        var fileMetadata = await _fileRepository.GetFileByIdAsync(fileId, userId, ct);

        if (fileMetadata == null)
        {
            throw new KeyNotFoundException($"File with ID {fileId} not found for user {userId}");
        }
        
        string sharedUrl = _blobService.GetReadOnlyLink(fileMetadata.BlobPath, TimeSpan.FromHours(1));
        
        return _mapper.Map<FileResponse>(fileMetadata) with 
        { 
            Url = sharedUrl 
        };
    }

    public async  Task<IEnumerable<FileResponse>> GetAllFilsAsync(Guid userId, CancellationToken ct = default)
    {
        var fileMataData = await _fileRepository.GetFilesByUserIdAsync(userId, ct);

        var dtos = _mapper.Map<IEnumerable<FileResponse>>(fileMataData);
        
        var response = dtos.Select(dto => dto with{ Url = _blobService.GetReadOnlyLink(dto.Id.ToString(), TimeSpan.FromHours(1))}).ToList();
        
        return response;
    }

    public async Task DownloadAsync(Guid fileId, Guid userId,CancellationToken ct = default)
    {
        var fileMetadata = await _fileRepository.GetFileByIdAsync(fileId, userId, ct);
    
        Stream destination  = new MemoryStream();
        
        destination.Position = 0;
        
        if (fileMetadata == null)
        {
            throw new KeyNotFoundException($"File with ID {fileId} not found for user {userId}");
        }
        
        await _blobService.DownloadAsync(fileMetadata.BlobPath, destination, ct );
    }
}