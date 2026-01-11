namespace FileService.Domain.Models;

public class UserFileMetadata
{
        public Guid Id { get; set; }
        public string FileName { get; set; }     
        public string OriginalName { get; set; }  
        public Guid UserId { get; set; }
        public string BlobPath { get; set; }
        public DateTime UploadedAt { get; set; }
        public bool IsDeleted { get; set; }
    
}