using BuildingBlocks.Redis.Events.Handler;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace BuildingBlocks.Redis.Events;

public class RedisSubscriberWorker : BackgroundService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IEnumerable<IScopedMessageHandler> _messageHandlers;
    
    public RedisSubscriberWorker(IConnectionMultiplexer connectionMultiplexer, IEnumerable<IScopedMessageHandler> messageHandlers)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _messageHandlers = messageHandlers;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _connectionMultiplexer.GetSubscriber();

        foreach (var handler in _messageHandlers)
        {
            await subscriber.SubscribeAsync(RedisChannel.Literal(handler.Channel), async (channel, message) =>
            {
                try
                {
                    await handler.HandleMessageAsync(message.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error processing message from channel {channel}: {ex.Message}", ex);
                }
            });
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}