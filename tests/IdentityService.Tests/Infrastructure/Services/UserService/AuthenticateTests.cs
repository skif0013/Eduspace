using FluentAssertions;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace IdentityService.Tests.Infrastructure.Services.UserService;

public class AuthenticateTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<RoleManager<RoleIdentity>> _roleManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IOutboxRepository> _outboxRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IdentityService.Infrastructure.Services.UserService _userService;

    public AuthenticateTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();

        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            new PasswordHasher<User>(),
            new IUserValidator<User>[] { },
            new IPasswordValidator<User>[] { },
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<User>>>()
        );
        
        var roleStoreMock = new Mock<IRoleStore<RoleIdentity>>();
        _roleManagerMock = new Mock<RoleManager<RoleIdentity>>(
            roleStoreMock.Object,
            new IRoleValidator<RoleIdentity>[] { },
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<ILogger<RoleManager<RoleIdentity>>>()
        );
        
        _unitOfWorkMock.Setup(u => u.OutboxRepository).Returns(_outboxRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Commit()).Returns(Task.CompletedTask);
        
        _userService = new IdentityService.Infrastructure.Services.UserService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _tokenServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }
    
    [Fact]
    public async Task AuthenticateAsync_ShouldReturnTokens_WhenCredentialsAreValid()
    {
        // Arrange
        var authRequest = new AuthRequest
        {
            Email = "test@example.com",
            Password = "P@ssw0rd1"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com"
        };

        var tokens = new TokenResponseDto
        {
            AccessToken = "access_token",
            RefreshToken = "refresh_token"
        };

        _userManagerMock.Setup(um => um.FindByEmailAsync(authRequest.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.IsEmailConfirmedAsync(user))
            .ReturnsAsync(true);

        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, authRequest.Password))
            .ReturnsAsync(true);

        _tokenServiceMock.Setup(ts => ts.GetTokens(It.IsAny<UserDto>()))
            .ReturnsAsync(tokens);

        // Act
        var result = await _userService.AuthenticateAsync(authRequest);

        // Assert
        result.IsError.Should().BeFalse();
        result.Data.AccessToken.Should().Be("access_token");
        result.Data.RefreshToken.Should().Be("refresh_token");

        _tokenServiceMock.Verify(ts => ts.GetTokens(
            It.Is<UserDto>(u =>
                u.Email == user.Email &&
                u.UserName == user.UserName &&
                u.Id == user.Id)), Times.Once);
    }
}