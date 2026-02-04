using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModified { get; set; }
}