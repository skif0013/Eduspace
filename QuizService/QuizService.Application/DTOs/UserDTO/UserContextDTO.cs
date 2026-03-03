namespace QuizService.Application.DTOs;

public class UserContextDTO
{
    public Guid UserId { get; set; }
    
    public string UserName { get; set; }
    
    public string Email { get; set; }

    public string[] Roles { get; set; } = Array.Empty<string>();
}