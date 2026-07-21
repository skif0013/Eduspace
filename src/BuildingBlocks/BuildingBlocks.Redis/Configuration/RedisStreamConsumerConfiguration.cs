namespace NotificationService.Infrastructure.Redis.Configuration;

/// <summary>
/// Value Object для конфигурации потребителя Redis Stream
/// Инкапсулирует параметры consumer group и stream
/// </summary>
public sealed class RedisStreamConsumerConfiguration
{
    public string StreamKey { get; }
    public string ConsumerGroupName { get; }
    public string ConsumerName { get; }

    public RedisStreamConsumerConfiguration(
        string streamKey,
        string consumerGroupName,
        string consumerName)
    {
        ValidateStreamKey(streamKey);
        ValidateConsumerGroupName(consumerGroupName);
        ValidateConsumerName(consumerName);

        StreamKey = streamKey;
        ConsumerGroupName = consumerGroupName;
        ConsumerName = consumerName;
    }

    private static void ValidateStreamKey(string streamKey)
    {
        if (string.IsNullOrWhiteSpace(streamKey))
            throw new ArgumentException("Stream key cannot be empty", nameof(streamKey));
    }

    private static void ValidateConsumerGroupName(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            throw new ArgumentException("Consumer group name cannot be empty", nameof(groupName));
    }

    private static void ValidateConsumerName(string consumerName)
    {
        if (string.IsNullOrWhiteSpace(consumerName))
            throw new ArgumentException("Consumer name cannot be empty", nameof(consumerName));
    }
}

