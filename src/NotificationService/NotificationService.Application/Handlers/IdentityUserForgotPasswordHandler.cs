using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using NotificationService.Infrastructure.Redis;
using BuildingBlocks.Redis.Events;

namespace NotificationService.Infrastructure.Service;

public class IdentityUserForgotPasswordHandler : ScopedMessageHandler
{
    public IdentityUserForgotPasswordHandler(IServiceScopeFactory scopeFactory)
        : base(scopeFactory)
    {
    }

    public override string Channel => "identity.user.forgot_password";

    protected override async Task HandleScopedAsync(string message, IServiceProvider serviceProvider)
    {
        var emailService = serviceProvider.GetRequiredService<IEmailService>();

        var quizEvent = DeserializeEvent(message);
        if (quizEvent is null)
            return;

        var emailDto = CreateEmailDto(quizEvent);
        await emailService.SendEmailAsync(emailDto);
    }

    private static UserResetPasswordEvent? DeserializeEvent(string message)
    {
        return JsonSerializer.Deserialize<UserResetPasswordEvent>(message);
    }

    private static EmailSendDTO CreateEmailDto(UserResetPasswordEvent resetPasswordEventEvent)
    {
        return new EmailSendDTO
        {
            To = resetPasswordEventEvent.UserEmail,
            Subject = "Reset Password",
            Body = resetPasswordEventEvent.Token
        }; 
    }
}