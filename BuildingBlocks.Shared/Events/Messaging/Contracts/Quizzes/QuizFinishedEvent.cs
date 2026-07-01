namespace BuildingBlocks.Redis.Events;

public record QuizFinishedEvent(
    Guid AttemptId,
    string UserEmail,
    string TotalScore,
    bool IsPassed) : IntegrationEvent(
    Guid.NewGuid(),
    DateTime.UtcNow);