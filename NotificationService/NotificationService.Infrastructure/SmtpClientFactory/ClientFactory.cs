using System.Net;
using System.Net.Mail;
using NotificationService.Application.Contracts;
using NotificationService.Domainn.Models;

namespace NotificationService.Infrastructure.SmtpClientFactory;

public class ClientFactory : IEmailCreateClient
{
    private readonly EmailSettings _emailSettings;

    public ClientFactory(EmailSettings emailSettings)
    {
        _emailSettings = emailSettings;
    }

    public SmtpClient CreateClient()
    {
        return new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
        {
            EnableSsl = _emailSettings.EnableSsl,
            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password)
        };
    }
}