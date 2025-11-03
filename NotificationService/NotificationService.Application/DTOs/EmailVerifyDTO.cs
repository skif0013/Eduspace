namespace NotificationService.Application.DTOs;

public class EmailVerifyDTO
{
    public string To { get; set; }
    
    public string UserName { get; set; }
    
    public string VerificationLink { get; set; }
    public string Code { get; set; }
}