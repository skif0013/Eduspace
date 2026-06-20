using System.Text.Json;
using StackExchange.Redis;

namespace BuildingBlocks.Shared.Events.Serialization;

public interface IStreamEventSerializer
{
    NameValueEntry[] SerializeEvent<T>(T @event) where T : IntegrationEvent;
    
    T DeserializeEvent<T>(NameValueEntry[] entries) where T : IntegrationEvent;
    
    string GetEventType(NameValueEntry[] entries);
}


