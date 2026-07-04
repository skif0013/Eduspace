using QuizService.Domain.Models;

namespace QuizService.Application.Contracts;

public interface IQuizIntegrationEventService
{
    //TODO: Fix
    Task PublishQuizStartedAsync(QuizAttempt attempt, string userToken);
    Task PublishQuizFinishedAsync(QuizAttempt attempt, string userToken);
}