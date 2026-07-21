using BuildingBlocks.Redis.Events.Handler;
using BuildingBlocks.Shared.Events.Messaging.Contracts.User;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using StackExchange.Redis;

namespace NotificationService.Application.Redis.EventHadnlers;

public class ConfirmEmailHandler : ScopedMessageHandler<EmailVerifyEvent>
{
    public ConfirmEmailHandler(IServiceScopeFactory scopeFactory) 
        : base(scopeFactory)
    {
    }

    
    public override string Channel => "email:verify";
    
    
    protected override async Task HandleMessageAsync(EmailVerifyEvent @event, IServiceProvider serviceProvider)
    {
        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        
        var emailDTO = new EmailVerifyDTO()
        {
            To = @event.To,
            UserName = @event.UserName,
            VerificationLink = @event.VerificationLink, 
            Code = @event.Code
        };
        
        await emailService.SendVerifyEmailAsync(emailDTO);
    }
}