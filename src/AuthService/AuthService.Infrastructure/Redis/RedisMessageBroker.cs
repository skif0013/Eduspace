// AuthService.Infrastructure/Redis/RedisMessageBroker.cs
using AuthService.Application.Interfaces.Services;
using StackExchange.Redis;

namespace AuthService.Infrastructure.Redis;

public class RedisMessageBroker : IRedisMessageBroker
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ISubscriber _subscriber;

    public RedisMessageBroker(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _subscriber = _redis.GetSubscriber();
    }

    public async Task<bool> PublishAsync(string channel, string message)
    {
        try
        {
            await _subscriber.PublishAsync(RedisChannel.Literal(channel), message);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SubscribeAsync(string channel, Action<string, string> handler)
    {
        try
        {
            await _subscriber.SubscribeAsync(RedisChannel.Literal(channel), (ch, val) =>
                handler(ch.ToString(), val.ToString()));
            return true;
        }
        catch
        {
            return false;
        }
    }
}