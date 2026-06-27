using QuizService.Domain.Models;

namespace QuizService.Application.Contracts;

public interface IQuizIntegrationEventService
{
    Task PublishQuizFinishedAsync(QuizAttempt attempt, string userToken);
}