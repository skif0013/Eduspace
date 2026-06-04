using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using NotificationService.Infrastructure.Redis;
using BuildingBlocks.Redis.Events;

namespace NotificationService.Infrastructure.Service;
public class IdentityUserCreatedHandler : ScopedMessageHandler
{
    public IdentityUserCreatedHandler(IServiceScopeFactory scopeFactory)
        : base(scopeFactory)
    {
    }

    public override string Channel => "identity.user.created";

    protected override async Task HandleScopedAsync(string message, IServiceProvider serviceProvider)
    {
        var emailService = serviceProvider.GetRequiredService<IEmailService>();

        var quizEvent = DeserializeEvent(message);
        if (quizEvent is null)
            return;

        var emailDto = CreateEmailDto(quizEvent);
        await emailService.SendVerifyEmailAsync(emailDto);
    }

    private static UserCreatedEvent? DeserializeEvent(string message)
    {
        return JsonSerializer.Deserialize<UserCreatedEvent>(message);
    }

    private static EmailVerifyDTO CreateEmailDto(UserCreatedEvent quizEvent)
    {
        return new EmailVerifyDTO
        {
            To = quizEvent.To,
            UserName = quizEvent.UserName,
            VerificationLink = quizEvent.VerificationLink,
            Code = quizEvent.Code
        };
    }
    
}
