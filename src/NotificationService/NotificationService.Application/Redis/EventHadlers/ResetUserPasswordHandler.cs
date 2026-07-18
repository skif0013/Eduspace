using BuildingBlocks.Redis.Events;
using BuildingBlocks.Redis.Events.Handler;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Redis.EventHadnlers;

public class ResetUserPasswordHandler : ScopedMessageHandler<UserResetPasswordEvent>
{
    public ResetUserPasswordHandler(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
    }


    public override string Channel => "user:reset:password";
    
    protected override Task HandleMessageAsync(UserResetPasswordEvent @event, IServiceProvider serviceProvider)
    {
        var emailService = serviceProvider.GetRequiredService<IEmailService>();

        var emailDTO = new ResetPasswordDTO(
            @event.To,
            @event.Token
        );
        
        return emailService.SendResetPasswordEmailAsync(emailDTO);
    }
}