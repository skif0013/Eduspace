using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories;

public class TokenRepository : Repository<RefreshToken>, ITokenRepository
{
    public TokenRepository(ApplicationDbContext context) : base(context) 
    { 
    }
    
    public Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
    {
        return _dbSet
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
    }
}