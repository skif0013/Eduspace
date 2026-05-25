namespace BuildingBlocks.Redis.Exceptions;

public class StreamProcessingException : Exception
{
    public StreamProcessingException(string message) : base(message) { }
    public StreamProcessingException(string message, Exception innerException) : base(message, innerException) { }
}

public class ConsumerGroupInitializationException : Exception
{
    public ConsumerGroupInitializationException(string groupName, string streamKey, Exception innerException)
        : base($"Failed to initialize consumer group '{groupName}' for stream '{streamKey}'", innerException) { }
}

public class EventPublishingException : Exception
{
    public EventPublishingException(string streamKey, Exception innerException)
        : base($"Failed to publish event to stream '{streamKey}'", innerException) { }
}

