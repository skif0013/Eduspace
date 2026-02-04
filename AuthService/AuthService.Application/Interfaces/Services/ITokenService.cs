using AuthService.Application.DTOs;
using AuthService.Domain.Results;

namespace AuthService.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResponseDto> GetTokens(UserDto userDto);
    Task<Result<bool>> RevokeRefreshTokenAsync(RefreshTokenRequest request);
    Task<Result<TokenResponseDto>> GetNewAccessTokenAsync(RefreshTokenRequest request);
}