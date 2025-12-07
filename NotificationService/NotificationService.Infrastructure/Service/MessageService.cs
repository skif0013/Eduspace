using System.Text.Json;
using NotificationService.Application.Interfaces.Services;
using NotificationService.Infrastructure.Redis;

namespace NotificationService.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly RedisMessageBroker _redisMessageBroker;
    public MessageService(RedisMessageBroker redisMessageBroker)
    {
        _redisMessageBroker = redisMessageBroker;
    }

    public async Task SendMessageAsync<T>(string actiom ,T message)
    {
        var res = await _redisMessageBroker.Publish(actiom, JsonSerializer.Serialize(message));
        if(res is true)
        {
            return ;
        }
        return ;    
    }
}