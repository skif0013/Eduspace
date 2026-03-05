using Microsoft.EntityFrameworkCore;
using QuizService.Application.Contracts.IQuizAttempt;
using QuizService.Domain.Enum;
using QuizService.Domain.Models;
using QuizService.Infrastructure.Data;

namespace QuizService.Infrastructure.Repositories;

public class AttemptRepository : IAttemptRepository
{
    private readonly ApplicationDbContext _context;

    public AttemptRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(QuizAttempt attempt)
    {
        await _context.QuizAttempts.AddAsync(attempt);
    }

    public async Task<QuizAttempt?> GetByIdAsync(Guid attemptId)
    {
        return await _context.QuizAttempts
            .Include(a => a.Answers)      
            .Include(a => a.Quiz)         
            .ThenInclude(q => q.Questions) 
            .FirstOrDefaultAsync(a => a.Id == attemptId);
    }

    public Task<QuizAttempt?> GetWithAnswersAsync(Guid attemptId)
    {
        return _context.QuizAttempts
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == attemptId);
    }

    public Task<bool> HasActiveAttemptAsync(Guid quizId, Guid userId)
    {
        return _context.QuizAttempts
            .AnyAsync(a =>
                a.QuizId == quizId &&
                a.UserId == userId &&
                a.Status == AttemptStatus.InProgress);
    }
}