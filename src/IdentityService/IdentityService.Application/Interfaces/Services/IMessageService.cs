using Shared.Messages;

namespace IdentityService.Application.Interfaces.Services;

public interface IMessageService
{
    Task SendMessageAsync<T>(string action, T message);
}