using BuildingBlocks.Redis.Contracts.Broker;
using BuildingBlocks.Redis.Contracts.Serealizer;
using BuildingBlocks.Redis.Events;
using StackExchange.Redis;

namespace NotificationService.Infrastructure.Redis.RedisBroker;

public class RedisMessageBroker : IRedisMessageBroker
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IStreamEventSerializer _eventSerializer;
    
    public RedisMessageBroker(IConnectionMultiplexer connectionMultiplexer, IStreamEventSerializer eventSerializer)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _eventSerializer = eventSerializer;
    }

    public async Task PublishAsync<T>(string streamKey, T @event) where T : IntegrationEvent
    {
        var database =  _connectionMultiplexer.GetDatabase();
        
        NameValueEntry[] redisFields = _eventSerializer.SerializeEvent(@event);
        
        // messageid -> id 
        await database.StreamAddAsync(streamKey, redisFields, messageId: "*");
    }
}