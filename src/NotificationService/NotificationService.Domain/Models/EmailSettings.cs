namespace NotificationService.Domain.Models;

public class EmailSettings
{
    public string SmtpHost { get; set; } 
    public int SmtpPort { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string Username { get; set; } 
    public string Password { get; set; } 
    
    public string FromAddress { get; set; }
}