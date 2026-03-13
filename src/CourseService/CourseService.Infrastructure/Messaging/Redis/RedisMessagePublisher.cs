using CourseService.Application.Messaging;
using StackExchange.Redis;

namespace CourseService.Infrastructure.Messaging.Redis;

public class RedisMessagePublisher : IMessagePublisher
{
    private readonly IConnectionMultiplexer _multiplexer;

    public RedisMessagePublisher(IConnectionMultiplexer multiplexer)
    {
        _multiplexer = multiplexer;
    }

    public async Task PublishAsync(string channel, string message)
    {
        var subscriber = _multiplexer.GetSubscriber();
        await subscriber.PublishAsync(channel, message);
    }
}
