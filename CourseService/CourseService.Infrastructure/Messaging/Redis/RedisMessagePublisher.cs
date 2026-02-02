using CourseService.Application.Messaging;

namespace CourseService.Infrastructure.Messaging.Redis;

public class RedisMessagePublisher : IMessagePublisher
{
    private readonly RedisMessageBroker _broker;

    public RedisMessagePublisher(RedisMessageBroker broker)
    {
        _broker = broker;
    }

    public Task<bool> PublishAsync(string channel, string message)
    {
        return _broker.Publish(channel, message);
    }
}
