using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using NotificationService.Infrastructure.Redis;
using Shared.Messages;

namespace NotificationService.Infrastructure.Services;

public class ConfimEmailHandler : ScopedMessageHandler
{
    public ConfimEmailHandler(
        IServiceScopeFactory scopeFactory)
        : base(scopeFactory)
    {
    }

    public override string Channel => "email:verify";

    protected override async Task HandleScopedAsync(string message, IServiceProvider serviceProvider)
    {
        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        
        var data = JsonSerializer.Deserialize<EmailVerifyEvent>(message);
        var emailDTO = new EmailVerifyDTO()
        {
            To = data.To,
            UserName = data.UserName,
            VerificationLink = "",
            Code = data.Code
        };

        await emailService.SendVerifyEmailAsync(emailDTO);
    }
}
