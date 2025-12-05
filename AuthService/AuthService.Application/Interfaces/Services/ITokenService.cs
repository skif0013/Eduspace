using AuthService.Application.DTOs;

namespace AuthService.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResponseDto> CreateAccessTokenAsync(UserDto userDto);
}