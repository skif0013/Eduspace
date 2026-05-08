using System.Text.Json;
using StackExchange.Redis;

namespace BuildingBlocks.Redis.Serialization;

public interface IStreamEventSerializer
{
    NameValueEntry[] SerializeEvent<T>(T @event) where T : class;
    T DeserializeEvent<T>(NameValueEntry[] entries) where T : class;
}

public sealed class JsonStreamEventSerializer : IStreamEventSerializer
{
    private const string PayloadFieldName = "payload";
    private const string EventTypeFieldName = "eventType";

    public NameValueEntry[] SerializeEvent<T>(T @event) where T : class
    {
        var payload = JsonSerializer.Serialize(@event);
        var eventType = typeof(T).Name;

        return new[]
        {
            new NameValueEntry(PayloadFieldName, payload),
            new NameValueEntry(EventTypeFieldName, eventType)
        };
    }

    public T DeserializeEvent<T>(NameValueEntry[] entries) where T : class
    {
        var payloadEntry = entries.FirstOrDefault(x => x.Name == PayloadFieldName);

        if (payloadEntry.Value.IsNullOrEmpty)
            throw new InvalidOperationException($"Stream entry missing '{PayloadFieldName}' field");

        var deserialized = JsonSerializer.Deserialize<T>(payloadEntry.Value.ToString());
        
        return deserialized ?? throw new InvalidOperationException($"Failed to deserialize event of type '{typeof(T).Name}'");
    }
}

