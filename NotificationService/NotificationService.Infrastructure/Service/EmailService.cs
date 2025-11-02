using System.Net.Mail;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using NotificationService.Domainn.Models;

namespace NotificationService.Infrastructure.Service;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly IEmailCreateClient _emailCreateClient;
    
    public EmailService(EmailSettings emailSettings, IEmailCreateClient emailCreateClient)
    {
        _emailSettings = emailSettings;
        _emailCreateClient = emailCreateClient;
    }

    public async Task SendEmailAsync(EmailSendDTO dto)
    {
        using var client = _emailCreateClient.CreateClient();

        var emailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.FromAddress, _emailSettings.Username),
            Subject = dto.Subject,
            Body = dto.Body,
            IsBodyHtml = true
        };
        
        emailMessage.To.Add(dto.To);

        await client.SendMailAsync(emailMessage); 
    }

    public async Task SendVerifyEmailAsync(EmailVerifyDTO dto)
    {
        using var client = _emailCreateClient.CreateClient();

        var subject = "Verify your email address";

        var emailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.FromAddress, _emailSettings.Username),
            Subject = subject,
            Body = $"Ваш код подтверждения: <b>{dto.Code}</b>",
            IsBodyHtml = true
        };
        
        emailMessage.To.Add(dto.To);

        await client.SendMailAsync(emailMessage); 
    }
}