namespace IdentityService.Application.Interfaces;

public interface IUserContext
{
        Guid UserId { get; }
        string Name { get; }
        string Email { get; }
}