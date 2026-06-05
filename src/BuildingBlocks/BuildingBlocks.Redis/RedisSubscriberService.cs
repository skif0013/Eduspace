using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Infrastructure.Redis.Configuration;
using StackExchange.Redis;

namespace NotificationService.Infrastructure.Redis;

public class RedisStreamSubscriberService : BackgroundService //TODO: переделать бекграунд сервис 
{
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly IEnumerable<IMessageHandler> _handlers;
    private readonly RedisStreamConsumerConfiguration _configuration;
    private readonly ILogger<RedisStreamSubscriberService> _logger;

    private const int MessageReadBatchSize = 10;
    private const int PollDelayMilliseconds = 1000;

    public RedisStreamSubscriberService(
        IConnectionMultiplexer multiplexer,
        IEnumerable<IMessageHandler> handlers,
        RedisStreamConsumerConfiguration configuration,
        ILogger<RedisStreamSubscriberService> logger)
    {
        _multiplexer = multiplexer;
        _handlers = handlers;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeConsumerGroupAsync();
        _logger.LogInformation("Redis Stream Subscriber started for stream: {StreamKey}", _configuration.StreamKey);

        await ConsumeMessagesAsync(stoppingToken);
    }

    private async Task InitializeConsumerGroupAsync()
    {
        var db = _multiplexer.GetDatabase();

        try
        {
            await db.StreamCreateConsumerGroupAsync(
                _configuration.StreamKey,
                _configuration.ConsumerGroupName,
                "$",
                createStream: true);

            _logger.LogInformation(
                "Consumer group {Group} created for stream {Stream}",
                _configuration.ConsumerGroupName,
                _configuration.StreamKey);
        }
        catch (RedisServerException ex)
            when (ex.Message.Contains("BUSYGROUP"))
        {
            _logger.LogInformation(
                "Consumer group {Group} already exists for stream {Stream}",
                _configuration.ConsumerGroupName,
                _configuration.StreamKey);
        }
    }

    private async Task ConsumeMessagesAsync(CancellationToken stoppingToken)
    {
        var db = _multiplexer.GetDatabase();

        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = await ReadGroupMessagesAsync(db);

            if (messages.Length == 0)
            {
                await Task.Delay(PollDelayMilliseconds, stoppingToken);
                continue;
            }

            await ProcessMessagesAsync(messages, db);
        }
    }

    private async Task<StreamEntry[]> ReadGroupMessagesAsync(IDatabase db)
    {
        return await db.StreamReadGroupAsync(
            _configuration.StreamKey,
            _configuration.ConsumerGroupName,
            _configuration.ConsumerName,
            count: MessageReadBatchSize,
            noAck: false);
    }

    private async Task ProcessMessagesAsync(StreamEntry[] entries, IDatabase db)
    {
        foreach (var entry in entries)
        {
            await ProcessSingleMessageAsync(entry, db);
        }
    }

    private async Task ProcessSingleMessageAsync(StreamEntry entry, IDatabase db)
    {
        var payload = ExtractPayloadFromEntry(entry);
        if (string.IsNullOrEmpty(payload))
        {
            _logger.LogWarning("Stream entry {EntryId} has no payload", entry.Id);
            return;
        }

        var handler = FindHandlerForStream();
        if (handler is null)
        {
            _logger.LogWarning("No handler found for stream {StreamKey}", _configuration.StreamKey);
            return;
        }

        await handler.HandleAsync(payload);
        _logger.LogDebug("Message {EntryId} processed successfully", entry.Id);

        await db.StreamAcknowledgeAsync(_configuration.StreamKey, _configuration.ConsumerGroupName, entry.Id);
    }

    private static string? ExtractPayloadFromEntry(StreamEntry entry)
    {
        var payloadEntry = entry.Values.FirstOrDefault(x => x.Name == "payload");
        return payloadEntry.Value.IsNullOrEmpty ? null : payloadEntry.Value.ToString();
    }

    private IMessageHandler? FindHandlerForStream()
    {
        return _handlers.FirstOrDefault(h => h.Channel == _configuration.StreamKey);
    }
}

