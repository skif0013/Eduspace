using QuizService.Application.Contracts;
using QuizService.Domain.Models;

namespace QuizService.Infrastructure.Services;

public class NoOpQuizIntegrationEventService : IQuizIntegrationEventService
{
    public Task PublishQuizFinishedAsync(QuizAttempt attempt, string userToken)
    {
        return Task.CompletedTask;
    }
}
