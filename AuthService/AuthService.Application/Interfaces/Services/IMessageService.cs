using Shared.Messages;

namespace AuthService.Application.Interfaces.Services;

public interface IMessageService
{
    Task<CreateUserDTO> SendMessageAsync(string action, CreateUserDTO message);
}