namespace NotificationService.Application.DTOs;

public class EmailSendDTO
{
    public string To { get; set; }
    
    public string Subject { get; set; }
    
    public string Body { get; set; }
    
}