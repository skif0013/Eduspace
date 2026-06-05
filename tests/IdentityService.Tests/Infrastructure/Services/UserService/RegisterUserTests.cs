using System.Threading.Tasks;
using FluentAssertions;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Identity;
using IdentityService.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace IdentityService.Tests.Infrastructure.Services.UserService;

public class RegisterUserTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<RoleManager<RoleIdentity>> _roleManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IOutboxRepository> _outboxRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IdentityService.Infrastructure.Services.UserService _userService;

    public RegisterUserTests()
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
    public async Task RegisterUser_WithCorrectValues_CreateUserCorrectly()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "P@ssw0rd1"
        };

        // Настроим CreateAsync возвращать успех
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success)
            .Verifiable();

        // Настроим GenerateEmailConfirmationTokenAsync возвращать токен
        _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
            .ReturnsAsync("confirm-token")
            .Verifiable();

        // OutboxRepository.AddAsync должен успешно завершаться
        _outboxRepoMock.Setup(r => r.AddAsync(It.IsAny<OutboxMessage>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _userService.RegisterAsync(dto);

        // Assert
        result.IsError.Should().BeFalse();

        _userManagerMock.Verify(um => um.CreateAsync(
            It.Is<User>(u => u.Email == dto.Email && u.UserName == dto.UserName), dto.Password), Times.Once);

        _userManagerMock.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()), Times.Once);

        _outboxRepoMock.Verify(r => r.AddAsync(It.IsAny<OutboxMessage>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_WithEmptyEmail_ThrowArgumentException()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            UserName = "testuser",
            Email = "",
            Password = "P@ssw0rd1"
        };
    
        // Настроим CreateAsync возвращать ошибку
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Email is required" }))
            .Verifiable();

        // Act
        var result = await _userService.RegisterAsync(dto);

        // Assert
        result.IsError.Should().BeTrue();
        result.ErrorMessage.Should().Contain("Email is required");

        // Проверяем, что CreateAsync был вызван
        _userManagerMock.Verify(um => um.CreateAsync(
            It.Is<User>(u => u.Email == dto.Email && u.UserName == dto.UserName), dto.Password), Times.Once);

        // НЕ проверяем GenerateEmailConfirmationTokenAsync — он вообще не должен быть вызван!
        _userManagerMock.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()), Times.Never);
    
        // Outbox тоже не должен быть вызван
        _outboxRepoMock.Verify(r => r.AddAsync(It.IsAny<OutboxMessage>()), Times.Never);
    
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }
}