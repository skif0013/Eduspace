using Microsoft.EntityFrameworkCore;
using QuizService.Domain.Models;

namespace QuizService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) {}
    
    public DbSet<Quiz> Quizzes { get; set; }
    
    public DbSet<Question> Questions { get; set; }
    
    public DbSet<AnswerOption> AnswerOptions { get; set; }
    
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(q => q.QuizId);
            entity.Property(q => q.Name).IsRequired().HasMaxLength(200);
            entity.Property(q => q.Description).HasMaxLength(1000);
            entity.HasMany(q => q.Questions).WithOne().OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Text).IsRequired();
            entity.HasMany<AnswerOption>().WithOne().OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AnswerOption>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Text).IsRequired();
        });
    }
}