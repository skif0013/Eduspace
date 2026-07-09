using System.Text.Json;
using BuildingBlocks.Redis.Events.Handler;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messages;

namespace IdentityService.Infrastructure.Services;

public  abstract class UserUpdatedHandler : ScopedMessageHandler
{
    public UserUpdatedHandler(
        IServiceScopeFactory scopeFactory)
        : base(scopeFactory)
    {
    }

    public override string Channel => "user:updated";

    //ovveride
    protected async Task HandleScopedAsync(string message, IServiceProvider serviceProvider)
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