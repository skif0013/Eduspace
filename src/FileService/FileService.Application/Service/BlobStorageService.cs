using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FileService.Application.Contracts.Repositories;
using FileService.Application.DTOs.BlobDTOs;


namespace FileService.Application.Service;

public class BlobStorageService : IBlobService
{
   private readonly BlobContainerClient _containerClient;
   
   public BlobStorageService(BlobServiceClient blobServiceClient)
   {
       _containerClient = blobServiceClient.GetBlobContainerClient("uploads");
   }


   public async Task<BlobUploadResult> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
   {
       var blobClient = _containerClient.GetBlobClient(fileName);

       var options = new BlobUploadOptions()
       {
           HttpHeaders = new BlobHttpHeaders()
           {
               ContentType = contentType
           }
       };
       
       await blobClient.UploadAsync(stream, options, ct);
       
       var sasUrl = GetReadOnlyLink(fileName, TimeSpan.FromHours(1));
       
       return new BlobUploadResult(fileName, sasUrl);
   }

   public async Task<bool> DeleteAsync(string blobPath, CancellationToken ct = default)
   {
        var response = await _containerClient.DeleteBlobIfExistsAsync(blobPath, cancellationToken: ct);   
        
        return response.Value;
   }

   public string GetReadOnlyLink(string blobPath, TimeSpan expiry)
   {
       var blobClient = _containerClient.GetBlobClient(blobPath);

       if (!blobClient.CanGenerateSasUri)
           throw new InvalidOperationException("check string connection");

       var sasBuilder = new BlobSasBuilder()
       {
           BlobContainerName = _containerClient.Name,
           BlobName = blobClient.Name,
           Resource = "b",
           ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
       };
       
       sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);
       
       return blobClient.GenerateSasUri(sasBuilder).ToString();
   }
}