using BuildingBlocks.Redis.Events;

namespace BuildingBlocks.Redis.Contracts.Broker;

public interface IRedisMessageBroker
{ 
    Task PublishAsync<T>(string streamKey, T @event) where T : IntegrationEvent;
}