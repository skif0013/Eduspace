using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using QuizService.Application.Contracts;
using QuizService.Application.DTOs;

namespace QuizService.Application.Services;

public class TokenService : ITokenService
{
    public UserContextDTO GetUserFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        var rolesClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();

        
        return new UserContextDTO
        {
            UserId = Guid.Parse(userIdClaim?.Value ?? throw new Exception("UserId claim is missing")),
            Email = emailClaim?.Value ?? throw new Exception("Email claim is missing"),
            Roles = rolesClaims
        };
    }


    public Guid GetUserIdFromToken(string token)
    {
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            throw new Exception("UserId claim is missing");
        }
        return Guid.Parse(userIdClaim.Value);
    }
}