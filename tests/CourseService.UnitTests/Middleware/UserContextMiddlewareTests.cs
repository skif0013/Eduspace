using System.Security.Claims;
using BuildingBlock.UserContextMiddleware.Middleware;
using BuildingBlock.UserContextMiddleware.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace CourseService.UnitTests.Middleware;

public class UserContextMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldPopulateUserContext_WhenRequestIsAuthenticated()
    {
        var userId = Guid.NewGuid();
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("userId", userId.ToString()),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "test@example.com")
            ], "Test"))
        };
        var userContext = new UserContext();
        var nextCalled = false;
        var middleware = new UserContextMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context, userContext);

        nextCalled.Should().BeTrue();
        userContext.UserId.Should().Be(userId);
        userContext.Name.Should().Be("Test User");
        userContext.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task InvokeAsync_ShouldLeaveUserContextUntouched_WhenRequestIsAnonymous()
    {
        var userContext = new UserContext();
        var nextCalled = false;
        var middleware = new UserContextMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(new DefaultHttpContext(), userContext);

        nextCalled.Should().BeTrue();
        userContext.UserId.Should().Be(Guid.Empty);
        userContext.Name.Should().BeNull();
        userContext.Email.Should().BeNull();
    }

    [Fact]
    public async Task InvokeAsync_ShouldThrowUnauthorizedAccessException_WhenUserIdIsInvalid()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim("userId", "invalid-id")], "Test"))
        };
        var middleware = new UserContextMiddleware(_ => Task.CompletedTask);

        var action = () => middleware.InvokeAsync(context, new UserContext());

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
