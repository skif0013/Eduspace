using BuildingBlocks.Redis.Events.Handler;
using BuildingBlocks.Shared.Events.Messaging.Contracts.User;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using StackExchange.Redis;

namespace NotificationService.Application.Redis.EventHadlers;

public class ConfirmEmailHandler : ScopedMessageHandler<EmailVerifyEvent>
{
    public ConfirmEmailHandler(IServiceScopeFactory scopeFactory) 
        : base(scopeFactory)
    {
    }

    // Имя канала строго совпадает с тем, что мы записали в Outbox Type ("email:verify")
    public override string Channel => "email:verify";
    
    // Метод принимает уже готовый, десериализованный объект @event
    protected override async Task HandleMessageAsync(EmailVerifyEvent @event, IServiceProvider serviceProvider)
    {
        // Достаем из локального scope сервис отправки писем
        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        
        // Маппим данные из ивента в DTO для отправки
        var emailDTO = new EmailVerifyDTO()
        {
            To = @event.To,
            UserName = @event.UserName,
            VerificationLink = @event.VerificationLink, // Если ссылка генерируется на стороне Identity
            Code = @event.Code
        };

        // Отправляем письмо
        await emailService.SendVerifyEmailAsync(emailDTO);
    }
}