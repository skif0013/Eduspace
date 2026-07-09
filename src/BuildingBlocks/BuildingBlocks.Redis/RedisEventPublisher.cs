using System.Text.Json;
using StackExchange.Redis;

namespace BuildingBlocks.Redis;

public class RedisEventPublisher : IEventPublisher
{
    private readonly IConnectionMultiplexer _connection;
    public RedisEventPublisher(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }
    public Task PublishAsync<TEvent>(string channel, TEvent @event) where TEvent : class
    {
        var subscriber = _connection.GetSubscriber();
        
        string message = JsonSerializer.Serialize(@event);
        return subscriber.PublishAsync(RedisChannel.Literal(channel), message);
    }
}