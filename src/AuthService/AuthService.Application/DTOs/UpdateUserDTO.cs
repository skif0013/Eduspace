namespace AuthService.Application.DTOs;

public class UpdateUserDTO
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}