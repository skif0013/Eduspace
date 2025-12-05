using System.Text.Json;
using AuthService.Application.Interfaces.Services;

using AuthService.Infrastructure.Redis;
using Shared.Messages;


namespace AuthService.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly RedisMessageBroker _redisMessageBroker;
    public MessageService(RedisMessageBroker redisMessageBroker)
    {
        _redisMessageBroker = redisMessageBroker;
    }

    public async Task<CreateUserDTO> SendMessageAsync(string actiom ,CreateUserDTO message)
    {
        var res = await _redisMessageBroker.Publish(actiom, JsonSerializer.Serialize(message));
        if(res is true)
        {
            return message;
        }
        return null;
    }
}