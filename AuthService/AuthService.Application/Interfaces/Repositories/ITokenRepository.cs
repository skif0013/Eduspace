using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Repositories;

public interface ITokenRepository
{
    Task AddRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
    Task UpdateRefreshTokenAsync(RefreshToken token);
}