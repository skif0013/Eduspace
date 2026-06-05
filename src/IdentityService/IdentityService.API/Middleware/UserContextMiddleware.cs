using System.Security.Claims;
using IdentityService.Application.Common.Models;

namespace IdentityService.API.Middleware;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserContext userContext)
    {
        var userIdClaim = context.User.FindFirst("userId")?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            userContext.UserId = userId;
        }
        
        userContext.Name = context.User.FindFirst(ClaimTypes.Name)?.Value;
        userContext.Email = context.User.FindFirst(ClaimTypes.Email)?.Value;
        
        await _next(context);
    }
}