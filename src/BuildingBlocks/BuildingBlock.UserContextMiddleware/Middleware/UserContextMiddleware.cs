using System.Security.Claims;
using BuildingBlock.UserContextMiddleware.Models;
using Microsoft.AspNetCore.Http;
    
namespace BuildingBlock.UserContextMiddleware.Middleware;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context, UserContext userContext)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var userIdClaim = context.User.FindFirst("userId")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId)) 
            {
                userContext.UserId = userId;
            }
            else 
            {
                throw new UnauthorizedAccessException("Invalid userId claim.");
            }

            userContext.Name = context.User.FindFirst(ClaimTypes.Name)?.Value;
            userContext.Email = context.User.FindFirst(ClaimTypes.Email)?.Value;
        }
        
        await _next(context);
    }
}