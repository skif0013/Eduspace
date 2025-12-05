using System.Text.Json;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;

using AuthService.Infrastructure.Redis;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messages;

namespace AuthService.Infrastructure.Services;

public class UserUpdatedHandler : ScopedMessageHandler
{
    public UserUpdatedHandler(
        IServiceScopeFactory scopeFactory)
        : base(scopeFactory)
    {
    }

    public override string Channel => "user:updated";

    protected override async Task HandleScopedAsync(string message, IServiceProvider serviceProvider)
    {
        var userService = serviceProvider.GetRequiredService<IUserService>();
        
        var data = JsonSerializer.Deserialize<UpdateUserEvent>(message);
        var userDto = new UpdateUserDTO
        {
            Id = data.Id,
            Email = data.Email,
            UserName = data.UserName
        };

        await userService.UpdateUserAsync(userDto);
    }
    
}