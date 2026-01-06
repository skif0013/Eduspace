using Microsoft.EntityFrameworkCore;
using QuizService.Domain.Models;

namespace QuizService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) {}
    
    public DbSet<Quiz> Quizzes { get; set; }
    
    public DbSet<Question> Questions { get; set; }
    
    public DbSet<AnswerOption> AnswerOptions { get; set; }
    
}