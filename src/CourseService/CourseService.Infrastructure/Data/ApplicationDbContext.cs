using CourseService.Domain.Abstractions;
using CourseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {   }

    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseRating> CourseRatings { get; set; }
    public DbSet<Lesson> Lessons { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Course>()
            .HasMany(x => x.CourseRatings)
            .WithOne(x => x.Course)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Course>()
            .HasMany(x => x.Lessons)
            .WithOne(x => x.Course)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CourseRating>()
            .HasIndex(x => new { x.CourseId, x.UserId })
            .IsUnique();
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = null;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
                entry.Property(x => x.CreatedAt).IsModified = false;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}

