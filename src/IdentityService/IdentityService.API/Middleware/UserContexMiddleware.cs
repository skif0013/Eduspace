using System.Security.Claims;
using IdentityService.Application.Common.Models;

namespace IdentityService.API.Middleware;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserContextMiddleware> _logger;

    public UserContextMiddleware(RequestDelegate next, ILogger<UserContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, UserContext userContext)
    {
        // userId считываем как есть (он не мапится)
        var userIdClaim = context.User.FindFirst("userId")?.Value;
    
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            userContext.UserId = userId;
        
            // Email мапится в ClaimTypes.Email (длинная строка из твоего JSON)
            userContext.Email = context.User.FindFirst(ClaimTypes.Email)?.Value;
        }

        await _next(context);
    }
}