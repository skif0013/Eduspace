using QuizService.Domain.Models;

namespace QuizService.Application.Contracts;

public interface IUserAnswerRepository
{
    Task<UserAnswer> AddAsync(UserAnswer userAnswer);
    
    Task<IReadOnlyCollection<UserAnswer>> GetByAttemptIdAsync(Guid attemptId);
    
    Task RemoveAsync(UserAnswer userAnswer);
    
    Task<UserAnswer?> GetByIdAsync(Guid userAnswerId);
}