
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Results;
using AuthService.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AuthService.Domain;

namespace AuthService.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public TokenService(
        IConfiguration configuration,

        ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
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
    
    public async Task<TokenResponseDto> CreateAccessTokenAsync(UserDto user)
    {
        //var refreshToken = await GenereteRefreshTokenAsync();
        var accessToken = await CreateTokenAsync(user);



        return new TokenResponseDto
        {
            AccessToken = accessToken,
        };
    }

    /*public async Task<Result<RefreshTokenResponseDto>> UploadTokensAsync(string refreshToken)
    {
        var refreshTokenFromDb = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
        if (refreshTokenFromDb == null)
        {
            return Result<RefreshTokenResponseDto>.Failure("Invalid refresh token");
        }
        refreshTokenFromDb.DeletedAt = DateTime.UtcNow;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == refreshTokenFromDb.UserId);

        var newAccessToken = await CreateTokenAsync(user);
        var newRefreshToken = await GenereteRefreshTokenAsync();

        RefreshToken RefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(RefreshToken);
        await _context.SaveChangesAsync();

        return Result<RefreshTokenResponseDto>.Success(new RefreshTokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }*/
    

    private async Task<string> GenereteRefreshTokenAsync()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
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

        /*var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }*/

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
