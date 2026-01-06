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
    
    public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync() =>
        await _context.Quizzes.Include(g => g.Questions).ToListAsync();

    
    public async Task<Quiz> AddQuizAsync(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();
        
        return quiz;
    }

    
    
    public async Task<Quiz> UpdateQuizAsync(Guid QuizId)
    {
        var quiz = await _context.Quizzes.FindAsync(QuizId);
        if (quiz != null)
        {
            _context.Quizzes.Update(quiz);
        }
        return quiz;
    }
    
    
    public async Task<Quiz> DeleteQuizAsync(Guid QuizId)
    {
        var quiz = await _context.Quizzes.FindAsync(QuizId);
        if (quiz != null)
        {
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }
        return quiz;
    }
    
    public async Task<Quiz> FindByIdAsync(Guid QuizId) =>
        await _context.Quizzes.Include(g => g.Questions).FirstOrDefaultAsync(q => q.QuizId == QuizId);
    
}