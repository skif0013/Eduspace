using Azure.Storage.Blobs;
using FileService.Application.Contracts.Repositories;
using FileService.Application.DTOs.BlobDTOs;


namespace FileService.Application.Service;

public class BlobStorageService : IBlobService
{

   private readonly BlobContainerClient _containerClient;
   private readonly IFileRepository _fileRepository;
   private readonly IUnitOfWork _uow;
   
   public BlobStorageService(BlobContainerClient containerClient, IFileRepository fileRepository, IUnitOfWork uow)
   {
         _uow = uow;
       _fileRepository = fileRepository;
       
       
       _containerClient = _blobServiceClient.GetBlobContainerClient("uploads");
   }


   public Task<BlobUploadResult> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
   {
       
   }

   public Task<bool> DeleteAsync(string blobPath, CancellationToken ct = default)
   {
       
   }

   public string GetReadOnlyLink(string blobPath, TimeSpan expiry)
   {
       
   }
}