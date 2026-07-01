using BuildingBlocks.Redis.Contracts.Broker;
using BuildingBlocks.Redis.Events;
using QuizService.Application.Contracts;
using QuizService.Domain.Models;

namespace QuizService.Application.Orchestration;

public class QuizProcessingOrchestrator : IQuizIntegrationEventService
{
    private readonly IRedisMessageBroker _redisMessageBroker;
    private readonly ITokenService _tokenService;

    public QuizProcessingOrchestrator(IRedisMessageBroker redisMessageBroker)
    {
        _redisMessageBroker = redisMessageBroker;
    }


    public Task PublishQuizStartedAsync(QuizAttempt attempt, string userToken)
    {
        var userMail = _tokenService.GetUserEmailFromToken(userToken);

        var quizStartedEvent = new QuizStartedEvent(
            AttemptId: attempt.Id,
            UserEmail: userMail,
            QuizId: attempt.QuizId.ToString()
        );

        return _redisMessageBroker.PublishAsync("quiz-started-stream", quizStartedEvent);
    }

    public Task PublishQuizFinishedAsync(QuizAttempt attempt, string userToken)
    {
       var userMail = _tokenService.GetUserEmailFromToken(userToken);

       var quizFinishedAsync = new QuizFinishedEvent(
           AttemptId: attempt.Id,
           UserEmail: userMail,
           TotalScore: attempt.TotalScore.ToString(),
           IsPassed: attempt.IsPassed()
        );
       
       return _redisMessageBroker.PublishAsync("quiz-finished-stream", quizFinishedAsync);
    }
}