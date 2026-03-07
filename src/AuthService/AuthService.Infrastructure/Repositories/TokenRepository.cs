using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly ApplicationDbContext _context;
    public TokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public Task AddRefreshTokenAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        return _context.SaveChangesAsync();
    }
    
    public Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
    {
        return _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
    }
    
    public Task UpdateRefreshTokenAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        return _context.SaveChangesAsync();
    }
}