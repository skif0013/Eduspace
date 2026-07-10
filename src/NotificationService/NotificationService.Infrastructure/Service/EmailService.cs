using System.Net.Mail;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Service;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly IEmailCreateClient _emailCreateClient;
    private readonly EmailTemplates _emailTemplates;
    
    public EmailService(EmailSettings emailSettings, IEmailCreateClient emailCreateClient, EmailTemplates emailTemplates)
    {
        _emailSettings = emailSettings;
        _emailCreateClient = emailCreateClient;
        _emailTemplates = emailTemplates;
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
        
        var body = _emailTemplates.body
            .Replace("{VerificationCode}", dto.Code ?? "");
        
        var subject = "Verify your email address";

        var emailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.FromAddress, _emailSettings.Username),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        emailMessage.To.Add(dto.To);

        await client.SendMailAsync(emailMessage); 
    }

    public async Task SendResetPasswordEmailAsync(ResetPasswordDTO dto)
    {
        
        using var client = _emailCreateClient.CreateClient();
    
        
        var body = _emailTemplates.body
            .Replace("{ResetPasswordLink}", dto.Token ?? "");
    
        var subject = "Reset your password";

       
        using var emailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.FromAddress, _emailSettings.Username),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
    
        emailMessage.To.Add(dto.To);
        
        await client.SendMailAsync(emailMessage);
    }
}