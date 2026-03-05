namespace AuthService.Application.DTOs;

public class ConfirmEmailRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
}