using BuildingBlocks.Redis.Contracts.Serealizer;
using BuildingBlocks.Shared.Events.Infrastructure.Initializer.Contract;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Infrastructure.Redis.Configuration;
using StackExchange.Redis;

namespace BuildingBlocks.Redis.Infrastructure;

public abstract class BaseRedisStreamSubscriber : BackgroundService
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
        _streamInitializer = streamInitializer; 
        _multiplexer = multiplexer;
        _serializer = serializer;
        _configuration = configuration;
        Logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _streamInitializer.EnsureGroupExists(
            _configuration.ConsumerGroupName,
            _configuration.StreamKey);
        
        Logger.LogInformation("Redis Stream Subscriber started for stream: {StreamKey}", _configuration.StreamKey);

        var db = _multiplexer.GetDatabase();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await db.StreamReadGroupAsync(
                    _configuration.StreamKey,
                    _configuration.ConsumerGroupName,
                    _configuration.ConsumerName,
                    count: MessageReadBatchSize,
                    noAck: false);

                if (messages.Length == 0)
                {
                    await Task.Delay(PollDelayMilliseconds, stoppingToken);
                    continue;
                }

                foreach (var entry in messages)
                {
                    await ProcessSingleEntryAsync(db, entry, stoppingToken);
                }
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogError(ex, "Error while consuming messages from stream {StreamKey}", _configuration.StreamKey);
                await Task.Delay(PollDelayMilliseconds, stoppingToken);
            }
        } 
    }

    private async Task ProcessSingleEntryAsync(IDatabase db, StreamEntry entry, CancellationToken cancellationToken)
    {
        try
        {
            
            string eventType = _serializer.GetEventType(entry.Values);

           
            bool isProcessed = await ProcessMessageAsync(eventType, entry.Values, cancellationToken);

            
            if (isProcessed)
            {
                await db.StreamAcknowledgeAsync(_configuration.StreamKey, _configuration.ConsumerGroupName, entry.Id);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to process stream entry {EntryId}", entry.Id);
        }
    }

    
    protected abstract Task<bool> ProcessMessageAsync(string eventType, NameValueEntry[] entries, CancellationToken cancellationToken);
}