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

    public async Task<Quiz> GetQuizAsync(Guid QuizId) =>
        await _context.Quizzes.Include(g => g.Questions).FirstOrDefaultAsync(q => q.QuizId == QuizId);
    
    public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync() =>
        await _context.Quizzes.Include(g => g.Questions).ToListAsync();

    public async Task AddQuizAsync(Quiz quiz) => await _context.AddAsync(quiz);


    public async Task<Quiz> SaveQuizAsync(Quiz quiz) => _context.Quizzes;
    
    
    public async Task<Quiz> UpdateQuizAsync(Guid QuizId)
    {
        var quiz = await _context.Quizzes.FindAsync(QuizId);
        if (quiz != null)
        {
            _context.Quizzes.Update(quiz);
        }
        return quiz;
    }
    
}