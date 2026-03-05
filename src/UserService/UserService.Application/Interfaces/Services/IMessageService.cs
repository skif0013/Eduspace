using Shared.Messages;

namespace UserService.Application.Interfaces.Services;

public interface IMessageService
{
    Task SendMessageAsync<T>(string action, T message);
}