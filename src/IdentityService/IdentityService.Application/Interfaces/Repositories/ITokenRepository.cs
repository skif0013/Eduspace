using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces.Repositories;

public interface ITokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
}