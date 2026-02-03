using Microsoft.EntityFrameworkCore;
using QuizService.Application.Repositories;
using QuizService.Domain.Models;
using QuizService.Infrastructure.Data;

namespace QuizService.Infrastructure.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly ApplicationDbContext _context;
    
    public QuizRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IReadOnlyCollection<Quiz>> GetAllQuizzesAsync() =>
        await _context.Quizzes.Include(g => g.Questions).ToListAsync();

    
    public async Task AddQuizAsync(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
    }
    
    public async Task<Quiz?> FindByIdAsync(Guid QuizId) =>
        await _context.Quizzes.Include(g => g.Questions).FirstOrDefaultAsync(q => q.QuizId == QuizId);


    public Task RemoveAsync(Quiz quiz)
    {
        _context.Quizzes.Remove(quiz);
        return Task.CompletedTask;
    }
}