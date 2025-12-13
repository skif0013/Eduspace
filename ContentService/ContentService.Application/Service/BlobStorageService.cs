using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using ContentService.Application.Contracts.Repositories;
using ContentService.Application.DTOs.GroupDTOs;
using ContentService.Domain.Models;

namespace ContentService.Application.Service;

public class BlobStorageService
{
   private readonly BlobServiceClient _blobServiceClient;
   private readonly IFileRepository _fileRepository;
   private readonly IUnitOfWork _uow;
   
   public BlobStorageService(BlobServiceClient blobServiceClient, IFileRepository fileRepository, IUnitOfWork uow)
   {
         _uow = uow;
       _fileRepository = fileRepository;
       _blobServiceClient = blobServiceClient;
   }

    public async Task<string> UploadFileAsync(Stream fileStream, string originalName, Guid userId)
    {
        var fileName = $"{userId}/{Guid.NewGuid()}_{originalName}";
        var containerClient = _blobServiceClient.GetBlobContainerClient("uploads");
        await containerClient.CreateIfNotExistsAsync();


        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream,  true);

        var file = new UserFileMetadata()
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            OriginalName = originalName,
            UserId = userId,
            BlobPath = fileName,
            UploadedAt = DateTime.UtcNow
        };

        await _fileRepository.AddAsync(file);
        await _uow.SaveChangesAsync();
        
        return file.Id.ToString();
    }


    public async Task<string> GetFileLinkAsync(Guid fileId, Guid userId)
    {
        var file = await _fileRepository.GetFileByIdAsync(fileId, userId);

        if (file is null) throw new Exception("File not found");
        
        var containerClient = _blobServiceClient.GetBlobContainerClient("uploads");
        var blobClient = containerClient.GetBlobClient(file.FileName);
        
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };
        
        sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);
        
        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }
    
    public async Task<bool> DeleteFileAsync(Guid fileId, Guid userId)
    {
        
        var file = await _fileRepository.GetFileByIdAsync(fileId, userId);
        if (file is null) throw new Exception("File not found");

        var container = _blobServiceClient.GetBlobContainerClient("uploads");
        var blobClient = container.GetBlobClient(file.BlobPath);
        
        var deleted = await blobClient.DeleteIfExistsAsync();
        
        await _fileRepository.DeleteAsync(file);
        await  _uow.SaveChangesAsync();

        return deleted;
    }
}