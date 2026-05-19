using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<Result<bool>> RevokeRefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _tokenService.RevokeRefreshTokenAsync(request);
        return result;
    }
    
    [AllowAnonymous]
    [HttpPost("NewAccessToken")]
    public async Task<Result<TokenResponseDto>> GetNewAccessToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _tokenService.GetNewAccessTokenAsync(request);
        return result;
    }
}