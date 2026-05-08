using System.IdentityModel.Tokens.Jwt;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokensController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokensController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("RevokeRefreshToken")]
    public async Task<Result<bool>> RevokeRefreshToken([FromBody] RefreshTokenRequest request)
    {
        // Достаем токен из заголовка Authorization
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
        
            // Читаем токен игнорируя то, что он просрочен (без полной валидации)
            var jwtToken = handler.ReadJwtToken(token);
            var userIdString = jwtToken.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
        
            // Теперь у вас есть userIdString, даже если токен просрочен!
        }

        var result = await _tokenService.RevokeRefreshTokenAsync(request);
        return result;
    }
    
    [HttpPost("NewAccessToken")]
    public async Task<Result<TokenResponseDto>> GetNewAccessToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _tokenService.GetNewAccessTokenAsync(request);
        return result;
    }
}