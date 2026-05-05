using QuizService.Application.Contracts;
using QuizService.Application.DTOs.EventDTOs;
using QuizService.Application.Exceptions;
using QuizService.Infrastructure.Redis.Configuration;
using QuizService.Infrastructure.Redis.Serialization;
using StackExchange.Redis;

namespace QuizService.Infrastructure.Redis;

/// <summary>
/// Публикует события завершения квиза в Redis Stream
/// Отвечает за надёжную доставку событий в систему
/// </summary>
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
