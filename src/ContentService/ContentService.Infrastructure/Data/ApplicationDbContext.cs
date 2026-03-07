using Microsoft.EntityFrameworkCore;
using ContentService.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContentService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<GroupMember> GroupMembers { get; set; } = null!;
    
    public DbSet<UserFileMetadata> UserFileMetadatas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.GroupName).IsRequired();
            entity.HasMany(e => e.Members).WithOne(m => m.Group).HasForeignKey(m => m.GroupId);
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.UserId });
        });
        
        
        modelBuilder.Entity<UserFileMetadata>(entity =>
        {
            entity.HasKey(f => f.Id);

            entity.Property(f => f.FileName).IsRequired();
            entity.Property(f => f.BlobPath).IsRequired();
            entity.Property(f => f.UserId).IsRequired();
        });
    }
}