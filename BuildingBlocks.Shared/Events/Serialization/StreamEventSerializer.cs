using System.Text.Json;
using StackExchange.Redis;

namespace BuildingBlocks.Shared.Events.Serialization;

public sealed class StreamEventSerializer : IStreamEventSerializer
{
    
    private const string PayloadFieldName = "payload";
    private const string EventTypeFieldName = "eventType";
    
    public NameValueEntry[] SerializeEvent<T>(T @event) where T : IntegrationEvent
    {
        // Сериализуется весь объект (включая уникальные поля вроде QuizId И базовые Id/CreationDate)
        var payload = JsonSerializer.Serialize(@event);
        var eventType = typeof(T).Name;

        return new[]
        {
            new NameValueEntry(PayloadFieldName, payload),
            new NameValueEntry(EventTypeFieldName, eventType)
        };
    }

    public T DeserializeEvent<T>(NameValueEntry[] entries) where T : IntegrationEvent
    {
        var payloadEntry = entries.FirstOrDefault(e => e.Name == PayloadFieldName);
        
        var deserializedEvent = JsonSerializer.Deserialize<T>(payloadEntry.Value.ToString());
        
        return deserializedEvent ?? throw new InvalidOperationException($"Failed to deserialize event of type {typeof(T).Name}");
    }

    public string GetEventType(NameValueEntry[] entries)
    {
        var typeEntry = entries.FirstOrDefault(x => x.Name == EventTypeFieldName);
        
        if (typeEntry.Value.IsNullOrEmpty)
            throw new InvalidOperationException($"Stream entry missing '{EventTypeFieldName}' field");
            
        return typeEntry.Value.ToString();
    }
}