using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Domain.Results;
using AuthService.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ITokenRepository _tokenRepository;
    private readonly UserManager<User> _userManager;

    public TokenService(IConfiguration configuration, ITokenRepository tokenRepository, UserManager<User> userManager)
    {
        _configuration = configuration;
        _tokenRepository = tokenRepository;
        _userManager = userManager;
    }
    
    public async Task<TokenResponseDto> GetTokens(UserDto user)
    {
        var accessToken = await CreateTokenAsync(user);
        var refreshToken = await CreateRefreshTokenAsync(user.Id);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<string> CreateRefreshTokenAsync(Guid userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        
        await _tokenRepository.AddRefreshTokenAsync(refreshToken);
        
        return token;
    } 
    
    public async Task<Result<TokenResponseDto>> GetNewAccessTokenAsync(RefreshTokenRequest request)
    {
        var storedRefreshToken = await _tokenRepository.GetRefreshTokenAsync(request.RefreshToken);
        if (storedRefreshToken == null || storedRefreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Result<TokenResponseDto>.Failure("Invalid or expired refresh token.");
        }
        
        storedRefreshToken.Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        storedRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(30);
        
        await _tokenRepository.UpdateRefreshTokenAsync(storedRefreshToken);
        
        var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId.ToString());
        if (user == null)
        {
            return Result<TokenResponseDto>.Failure("User not found.");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };

        var accessToken = await CreateTokenAsync(userDto);
        
        return Result<TokenResponseDto>.Success(new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = storedRefreshToken.Token
        });
    }
    
    public async Task<Result<bool>> RevokeRefreshTokenAsync(RefreshTokenRequest request)
    {
        var RefreshToken = await _tokenRepository.GetRefreshTokenAsync(request.RefreshToken);
        if(RefreshToken == null)
        {
            return Result<bool>.Failure("Invalid refresh token.");
        }
        
        RefreshToken.ExpiresAt = DateTime.UtcNow;
        await _tokenRepository.UpdateRefreshTokenAsync(RefreshToken);
        
        return Result<bool>.Success(true);
    }
    
    private async Task<string> CreateTokenAsync(UserDto user)
    {
        int expirationMinutes = int.Parse(_configuration["JwtTokenSettings:ExparingTimeMinute"]!);
        var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);
        var token = CreateJwtToken(
            await CreateClaimsAsync(user),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
    
private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration) =>
    new(
        _configuration["JwtTokenSettings:ValidIssuer"], 
    _configuration["JwtTokenSettings:ValidAudience"],
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

private async Task<List<Claim>> CreateClaimsAsync(UserDto user)
   {
       var jwtSub = _configuration["JwtTokenSettings:JwtRegisteredClaimNamesSub"];
   
       var claims = new List<Claim>
       {
           new Claim("userId", user.Id.ToString()),
           new Claim(ClaimTypes.Name, user.UserName),
           new Claim(ClaimTypes.Email, user.Email),
       };
   
       var userEntity = await _userManager.FindByIdAsync(user.Id.ToString());

       if (userEntity != null)
       {
           var roles = await _userManager.GetRolesAsync(userEntity);
           foreach (var role in roles)
           {
               claims.Add(new Claim(ClaimTypes.Role, role));
           }
       }
   
       return claims;
   }

    private SigningCredentials CreateSigningCredentials()
    {
        var symmetricSecurityKey = _configuration["JwtTokenSettings:SymmetricSecurityKey"];

        return new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricSecurityKey)),
            SecurityAlgorithms.HmacSha256
        );
    }
}
