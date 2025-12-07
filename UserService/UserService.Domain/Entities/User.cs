using UserService.Domain.Abstractions;

namespace UserService.Domain.Entities;

public class User : Entity
{
    public string UserName { get; set; }

    public string Email { get; set; }
    public string? AvatarURL { get; set; }
    public Guid UserId { get; set; }
}