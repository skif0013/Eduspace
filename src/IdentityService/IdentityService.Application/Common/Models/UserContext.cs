using IdentityService.Application.Interfaces;

namespace IdentityService.Application.Common.Models;

public class UserContext : IUserContext
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}