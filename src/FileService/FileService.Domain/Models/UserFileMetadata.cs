namespace FileService.Domain.Models;

public class UserFileMetadata
{
    public Guid Id { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public string Title { get; private set; }
    
    public string BlobPath { get; private set; }
    
    public long SizeInBytes { get; private set; }
    
    public string ContentType { get; private set; }
    
    public bool IsDeleted { get; private set; }
    
    public DateTime? DeletedAt { get; private set; }
    
    public DateTime CreatedAt { get; private set; }

    private UserFileMetadata() { } 

    public UserFileMetadata(Guid userId, string blobPath, long sizeInBytes, string contentType, string title)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required");
        
        if (string.IsNullOrWhiteSpace(blobPath)) throw new ArgumentException("Blob path is required");
        
        if (sizeInBytes <= 0) throw new ArgumentException("File cannot be empty");

        Id = Guid.NewGuid();
        UserId = userId;
        BlobPath = blobPath;
        SizeInBytes = sizeInBytes;
        ContentType = contentType;
        Title = title;
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void UpdateMetadata(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle)) 
            throw new Exception("Title cannot be empty");
            
        Title = newTitle;
    }
    
    public void ReplaceContent(string newBlobPath, long newSize)
    {
        if (string.IsNullOrWhiteSpace(newBlobPath)) 
            throw new Exception("New blob path is required");

        BlobPath = newBlobPath;
        SizeInBytes = newSize;
    }
}