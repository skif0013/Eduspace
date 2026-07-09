using BuildingBlocks.Redis.Events;

namespace BuildingBlocks.Shared.Events.Messaging.Contracts.User;

public record EmailVerifyEvent(
    string To,
    string UserName,
    string Code
    ) : IntegrationEvent(new Guid(), DateTime.UtcNow);