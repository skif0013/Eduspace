using Microsoft.Extensions.Logging;
using UserService.Application.Interfaces.Services;
using Microsoft.Extensions.Hosting;


namespace UserService.Infrastructure.Redis;

public class RedisSubscriberService : BackgroundService
{
    private readonly RedisMessageBroker _broker;
    private readonly IEnumerable<IMessageHandler> _handlers;

    public RedisSubscriberService(
        RedisMessageBroker broker,
        IEnumerable<IMessageHandler> handlers)
    {
        _broker = broker;
        _handlers = handlers;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var handler in _handlers)
        {
            await _broker.Subscribe(handler.Channel, async (channel, message) =>
            {
                await handler.HandleAsync(message.ToString());
            });
        }
    }
}
