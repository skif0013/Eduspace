using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CourseService.WebApi.Extentions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst("userId")
            ?? user.FindFirst(ClaimTypes.NameIdentifier)
            ?? user.FindFirst(JwtRegisteredClaimNames.Sub);

        if(userIdClaim == null)
        {
            throw new UnauthorizedAccessException("UserId claim is missing");
        }

        if(!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("UserId claim is not a valid Guid");
        }

        return userId;
    }
}
