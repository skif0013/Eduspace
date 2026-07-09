namespace BuildingBlocks.Redis;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(string channel, TEvent @event) where TEvent : class;
    
    Task PublishRawAsync(string channel, string rawJsonMessage);
}