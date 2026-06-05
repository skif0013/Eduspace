using System.Text.Json;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;

using IdentityService.Infrastructure.Redis;
using Shared.Messages;
using StackExchange.Redis;


namespace IdentityService.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly IDatabase _redisDatabase;
    public MessageService(IDatabase rD)
    {
        _redisDatabase = rD;
    }

    public async Task SendMessageAsync<T>(string action, T message)
    {
        var payload = message is string s
            ? s
            : JsonSerializer.Serialize(message);

        await _redisDatabase.StreamAddAsync(
            action,
            new NameValueEntry[]
            {
                new("payload", payload)
            });
    }
}