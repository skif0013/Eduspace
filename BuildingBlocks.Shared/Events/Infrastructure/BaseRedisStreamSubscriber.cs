using BuildingBlocks.Shared.Events.Infrastructure.Initializer.Contract;
using BuildingBlocks.Shared.Events.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Infrastructure.Redis.Configuration;
using StackExchange.Redis;

namespace BuildingBlocks.Shared.Events.Infrastructure;

public class BaseRedisStreamSubscriber : BackgroundService
{
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly IStreamEventSerializer _serializer;
    private readonly IRedisStreamInitializer _streamInitializer;
    private readonly RedisStreamConsumerConfiguration _configuration;
    protected readonly ILogger Logger;

    private const int MessageReadBatchSize = 10;
    private const int PollDelayMilliseconds = 1000;

    protected BaseRedisStreamSubscriber(
        IRedisStreamInitializer streamInitializer,
        IConnectionMultiplexer multiplexer,
        IStreamEventSerializer serializer,
        RedisStreamConsumerConfiguration configuration,
        ILogger logger)
    {
        _multiplexer = multiplexer;
        _serializer = serializer;
        _configuration = configuration;
        Logger = logger;
    }
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _streamInitializer.EnsureGroupExists(
            _configuration.StreamKey,
            _configuration.ConsumerGroupName);
        
        
        Logger.LogInformation("Redis Stream Subscriber started for stream: {StreamKey}", _configuration.StreamKey);

        var db = _multiplexer.GetDatabase();
        // ... дальше твой цикл while без изменений
    }
}