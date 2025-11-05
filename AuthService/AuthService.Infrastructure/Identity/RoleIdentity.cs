using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Identity;

public class RoleIdentity : IdentityRole<Guid>
{
    public ICollection<IdentityUserRole<Guid>>? UserRoles { get; set; }
}