using System.Net.Mail;

namespace NotificationService.Application.Contracts;

public interface IEmailCreateClient
{ 
  public  SmtpClient CreateClient();
}