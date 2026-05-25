using BuildingBlocks.Redis.Contracts;
using BuildingBlocks.Redis.Events;
using BuildingBlocks.Redis.Exceptions;
using QuizService.Infrastructure.Redis.Configuration;
using BuildingBlocks.Redis.Serialization;
using StackExchange.Redis;

namespace QuizService.Infrastructure.Redis;

public sealed class QuizFinishedEventStreamPublisher : IQuizFinishedEventPublisher
{
    private readonly IDatabase _database;
    private readonly RedisStreamPublisherConfiguration _configuration;
    private readonly IStreamEventSerializer _serializer;

    public QuizFinishedEventStreamPublisher(
        ConnectionMultiplexer multiplexer,
        RedisStreamPublisherConfiguration configuration,
        IStreamEventSerializer serializer)
    {
        _database = multiplexer.GetDatabase();
        _configuration = configuration;
        _serializer = serializer;
    }

    public async Task PublishAsync(QuizFinishedEvent @event, CancellationToken cancellationToken = default)
    {
        var entries = _serializer.SerializeEvent(@event);
        await PublishToStreamAsync(entries, cancellationToken);
    }

    private async Task PublishToStreamAsync(NameValueEntry[] entries, CancellationToken cancellationToken)
    {
        try
        {
            await _database.StreamAddAsync(_configuration.StreamKey, entries);
        }
        catch (RedisConnectionException ex)
        {
            throw new EventPublishingException(_configuration.StreamKey, ex);
        }
        catch (RedisException ex)
        {
            throw new EventPublishingException(_configuration.StreamKey, ex);
        }
    }
}
