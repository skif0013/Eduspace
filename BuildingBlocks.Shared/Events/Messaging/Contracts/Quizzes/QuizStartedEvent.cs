namespace BuildingBlocks.Redis.Events;

public record QuizStartedEvent(
    Guid AttemptId,
    string UserEmail,
    string QuizId) : IntegrationEvent(
    Guid.NewGuid(),
    DateTime.UtcNow);