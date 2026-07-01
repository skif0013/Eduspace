using QuizService.Domain.Models;

namespace QuizService.Application.Contracts;

public interface IQuizIntegrationEventService
{
    //TODO: Fix
    Task PublishQuizStartedAsync(Quiz quiz, string userToken);
    Task PublishQuizFinishedAsync(Quiz quiz, string userToken);
}