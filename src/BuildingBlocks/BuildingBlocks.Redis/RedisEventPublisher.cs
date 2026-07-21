using System.Text.Json;
using StackExchange.Redis;

namespace BuildingBlocks.Redis;

public class RedisEventPublisher : IEventPublisher
{
    private readonly ISubscriber _subscriber;
    public RedisEventPublisher(IConnectionMultiplexer connection)
    {
        _subscriber = connection.GetSubscriber();
    }
    public Task PublishAsync<TEvent>(string channel, TEvent @event) where TEvent : class
    {
        string message = JsonSerializer.Serialize(@event);
        
        return _subscriber.PublishAsync(RedisChannel.Literal(channel), message);
    }

    public async Task PublishRawAsync(string channel, string rawJsonMessage)
    { 
        await _subscriber.PublishAsync(RedisChannel.Literal(channel), rawJsonMessage);
    }
}