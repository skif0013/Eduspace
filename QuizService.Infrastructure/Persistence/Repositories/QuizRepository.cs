using Microsoft.EntityFrameworkCore;
using QuizService.Application.Repositories;
using QuizService.Domain.Models;
using QuizService.Infrastructure.Data;

namespace QuizService.Infrastructure.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;


    public QuizRepository(ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }
    
    public async Task<IReadOnlyCollection<Quiz>> GetAllQuizzesAsync() =>
        await _context.Quizzes.Include(g => g.Questions).ToListAsync();

    
    public async Task AddQuizAsync(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task<Quiz?> FindByIdAsync(Guid QuizId) =>
        await _context.Quizzes.Include(g => g.Questions).FirstOrDefaultAsync(q => q.QuizId == QuizId);


    public Task RemoveAsync(Quiz quiz)
    {
        _context.Quizzes.Remove(quiz);
        return Task.CompletedTask;
    }
}