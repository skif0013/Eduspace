namespace UserService.Application.DTO;

public class CreateUserDTO
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}