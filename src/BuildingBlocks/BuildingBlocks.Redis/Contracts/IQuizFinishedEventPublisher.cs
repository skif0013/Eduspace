namespace BuildingBlocks.Redis.Contracts;

public interface IQuizFinishedEventPublisher
{
    Task PublishAsync(Events.QuizFinishedEvent @event, CancellationToken cancellationToken = default);
}

