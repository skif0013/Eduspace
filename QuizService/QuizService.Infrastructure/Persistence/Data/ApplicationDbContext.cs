using Microsoft.EntityFrameworkCore;
using QuizService.Domain.Models;

namespace QuizService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) {}
    
    public DbSet<Quiz> Quizzes { get; set; }
    
    public DbSet<Question> Questions { get; set; }
    
    public DbSet<AnswerOption> AnswerOptions { get; set; }
    
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    
    public DbSet<UserAnswer> UserAnswers { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Name).IsRequired().HasMaxLength(200);
            entity.Property(q => q.Description).HasMaxLength(1000);
            entity.Property(q => q.Category).HasMaxLength(100);
            
            entity.Navigation(q => q.Questions)
                .HasField("_questions")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            
            entity.HasMany(q => q.Questions)
                .WithOne()
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.Ignore(q => q.MaxScore);
        });


        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(q => q.Id);


            entity.HasMany(q => q.AnswerOptions)
                .WithOne()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);


            var navigation = entity.Metadata.FindNavigation(nameof(Question.AnswerOptions));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        });
        
        modelBuilder.Entity<QuizAttempt>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.HasMany(a => a.Answers)
                .WithOne(ua => ua.Attempt)
                .HasForeignKey(ua => ua.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);


            entity.Navigation(a => a.Answers)
                .HasField("_answers")
                .UsePropertyAccessMode(PropertyAccessMode.Field);


            entity.Property(a => a.TotalScore).IsRequired();
        });


        modelBuilder.Entity<UserAnswer>(entity =>
        {
            entity.HasKey(ua => ua.Id);

            entity.HasIndex(ua => ua.QuestionId);


            entity.Property(ua => ua.SelectedOptionId)
                .HasColumnType("uuid[]");
        });
    }
}