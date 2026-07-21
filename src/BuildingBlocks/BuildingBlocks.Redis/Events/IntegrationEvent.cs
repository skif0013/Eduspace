namespace BuildingBlocks.Redis.Events;

public record IntegrationEvent(
    Guid Id,
    DateTime CreatedOn
    );