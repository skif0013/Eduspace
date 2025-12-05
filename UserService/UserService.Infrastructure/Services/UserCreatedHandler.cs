using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Messages;
using UserService.Application.Interfaces.Services;
using UserService.Infrastructure.Redis;

namespace UserService.Infrastructure.Services;

public class UserCreatedHandler : ScopedMessageHandler
{
    public UserCreatedHandler(
        IServiceScopeFactory scopeFactory)
        : base(scopeFactory)
    {
    }

    public override string Channel => "user:created";

    protected override async Task HandleScopedAsync(string message, IServiceProvider serviceProvider)
    {
        var userService = serviceProvider.GetRequiredService<IUserService>();
        
        var data = JsonSerializer.Deserialize<CreateUserDTO>(message);
        var userDto = new Application.DTO.CreateUserDTO
        {
            UserId = data.UserId,
            Email = data.Email,
            UserName = data.UserName
        };

        await userService.CreateUserAsync(userDto);
    }
}
