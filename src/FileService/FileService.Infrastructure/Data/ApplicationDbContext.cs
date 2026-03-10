using FileService.Application.Contracts.Repositories;
using FileService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<UserFileMetadata> UserFileMetadatas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserFileMetadata>(entity =>
        {
            entity.HasKey(f => f.Id);

            entity.Property(f => f.FileName).IsRequired();
            entity.Property(f => f.BlobPath).IsRequired();
            entity.Property(f => f.UserId).IsRequired();
        });
    }
}