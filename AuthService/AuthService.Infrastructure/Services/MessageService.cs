using System.Text.Json;
using AuthService.Application.Interfaces.Services;

using AuthService.Infrastructure.Redis;
using Shared.Messages;


namespace AuthService.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly IRedisMessageBroker _redisMessageBroker;
    public MessageService(IRedisMessageBroker redisMessageBroker)
    {
        _redisMessageBroker = redisMessageBroker;
    }

    public async Task SendMessageAsync<T>(string actiom ,T message)
    {
        var res = await _redisMessageBroker.PublishAsync(actiom, JsonSerializer.Serialize(message));
    }
}