namespace IdentityService.Infrastructure.BackgroundJobs;

public interface IOutboxEventPublisher
{
    Task PublishRawAsync(string channel, string rawJsonMessage);
}