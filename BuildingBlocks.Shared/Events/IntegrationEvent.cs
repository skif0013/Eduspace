namespace BuildingBlocks.Shared.Events;

public record IntegrationEvent(
    Guid Id,
    DateTime CreatedOn
    );