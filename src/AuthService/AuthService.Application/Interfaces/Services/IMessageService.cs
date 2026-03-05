using Shared.Messages;

namespace AuthService.Application.Interfaces.Services;

public interface IMessageService
{
    Task SendMessageAsync<T>(string action, T message);
}