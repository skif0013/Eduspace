using Microsoft.EntityFrameworkCore;
using QuizService.Application.Contracts;
using QuizService.Domain.Models;
using QuizService.Infrastructure.Data;

namespace QuizService.Infrastructure.Repositories;

public class UserAnswerRepository : IUserAnswerRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public async Task<UserAnswer> AddAsync(UserAnswer userAnswer)
    {
        await _dbContext.UserAnswers.AddAsync(userAnswer);
        
        return userAnswer;
    }

    public async Task<IReadOnlyCollection<UserAnswer>> GetByAttemptIdAsync(Guid attemptId)
    {
        return await _dbContext.UserAnswers
            .Where(ua => ua.AttemptId == attemptId)
            .ToListAsync();
    }

    public Task RemoveAsync(UserAnswer userAnswer)
    {
        _dbContext.UserAnswers.Remove(userAnswer);
        
        return Task.CompletedTask;
    }

    public Task<UserAnswer?> GetByIdAsync(Guid userAnswerId)
    {
        return _dbContext.UserAnswers
            .FirstOrDefaultAsync(ua => ua.Id == userAnswerId);
    }
}