using QuizService.Domain.Models;

namespace QuizService.Application.Contracts.IQuizAttempt;

public interface IAttemptRepository
{
    Task AddAsync(QuizAttempt attempt);

    Task<QuizAttempt?> GetByIdAsync(Guid attemptId);

    Task<QuizAttempt?> GetWithAnswersAsync(Guid attemptId);

    Task<bool> HasActiveAttemptAsync(Guid quizId, Guid userId);
}