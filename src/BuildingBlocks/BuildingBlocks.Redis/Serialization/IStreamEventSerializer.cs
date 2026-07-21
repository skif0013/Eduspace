using BuildingBlocks.Redis.Events;
using StackExchange.Redis;

namespace BuildingBlocks.Redis.Contracts.Serealizer;

public interface IStreamEventSerializer
{
    NameValueEntry[] SerializeEvent<T>(T @event) where T : IntegrationEvent;
    
    T DeserializeEvent<T>(NameValueEntry[] entries) where T : IntegrationEvent;
    
    string GetEventType(NameValueEntry[] entries);
}