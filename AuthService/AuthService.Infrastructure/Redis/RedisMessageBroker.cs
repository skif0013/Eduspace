using StackExchange.Redis;

namespace AuthService.Infrastructure.Redis;

public class RedisMessageBroker
{
    private readonly ConnectionMultiplexer _redis;
    private readonly ISubscriber _subscriber;
    public RedisMessageBroker(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _subscriber = _redis.GetSubscriber();
    }
    
    public async Task<bool> Publish(string channel, string message)
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
    
    public async Task<bool> Subscribe(string channel, Action<RedisChannel, RedisValue> handle)
    {
        try
        {
            await _subscriber.SubscribeAsync(RedisChannel.Literal(channel), handle);
            return true;
        }
        catch
        {
            return false;
        }
    }
}