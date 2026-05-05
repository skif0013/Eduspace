using QuizService.Application.DTOs.EventDTOs;

namespace QuizService.Application.Contracts;

public interface IQuizFinishedEventPublisher
{
    Task PublishAsync(QuizFinishedEvent @event, CancellationToken cancellationToken = default);
}

