// AuthService.Application/Interfaces/Services/IRedisMessageBroker.cs
namespace AuthService.Application.Interfaces.Services;

public interface IRedisMessageBroker
{
    Task<bool> PublishAsync(string channel, string message);
    Task<bool> SubscribeAsync(string channel, Action<string, string> handler);
}