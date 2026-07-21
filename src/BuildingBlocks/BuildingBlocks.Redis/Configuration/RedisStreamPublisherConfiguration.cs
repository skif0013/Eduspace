using StackExchange.Redis;

namespace QuizService.Infrastructure.Redis.Configuration;

/// <summary>
/// Value Object для конфигурации Redis Stream publisher
/// Инкапсулирует параметры подключения и поведение
/// </summary>
public sealed class RedisStreamPublisherConfiguration
{
    public string StreamKey { get; }
    public string EndpointAddress { get; }
    public string? Username { get; }
    public string? Password { get; }

    public RedisStreamPublisherConfiguration(
        string streamKey,
        string endpointAddress,
        string? username = null,
        string? password = null)
    {
        ValidateStreamKey(streamKey);
        ValidateEndpoint(endpointAddress);

        StreamKey = streamKey;
        EndpointAddress = endpointAddress;
        Username = username;
        Password = password;
    }

    public ConfigurationOptions BuildConfigurationOptions()
    {
        var options = new ConfigurationOptions
        {
            EndPoints = { EndpointAddress },
            AbortOnConnectFail = false
        };

        if (!string.IsNullOrEmpty(Username))
            options.User = Username;

        if (!string.IsNullOrEmpty(Password))
            options.Password = Password;

        return options;
    }

    private static void ValidateStreamKey(string streamKey)
    {
        if (string.IsNullOrWhiteSpace(streamKey))
            throw new ArgumentException("Stream key cannot be empty", nameof(streamKey));
    }

    private static void ValidateEndpoint(string endpoint)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
            throw new ArgumentException("Redis endpoint cannot be empty", nameof(endpoint));
    }
}

