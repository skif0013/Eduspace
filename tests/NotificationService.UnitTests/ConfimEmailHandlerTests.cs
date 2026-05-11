using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using NotificationService.Infrastructure.Services;

namespace NotificationService.UnitTests;

// Локальный класс события верификации email для тестирования
public record EmailVerifyEventTest(string To, string UserName, string Code);

/// <summary>
/// Модульные тесты для ConfimEmailHandler
/// Проверяют корректность обработки сообщений верификации email
/// </summary>
public class ConfimEmailHandlerTests
{
    /// <summary>
    /// Проверяет, что handler правильно десериализует сообщение
    /// и вызывает emailService.SendVerifyEmailAsync с корректными данными
    /// </summary>
    [Fact]
    public async Task HandleAsync_DeserializesMessageAndSendsVerificationEmail()
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();

        // Настраиваем цепочку моков
        mockScopeFactory
            .Setup(x => x.CreateScope())
            .Returns(mockServiceScope.Object);

        mockServiceScope
            .Setup(x => x.ServiceProvider)
            .Returns(mockServiceProvider.Object);

        mockServiceProvider
            .Setup(x => x.GetService(typeof(IEmailService)))
            .Returns(mockEmailService.Object);

        var handler = new ConfimEmailHandler(mockScopeFactory.Object);

        // Создаем тестовое событие
        var testEvent = new EmailVerifyEventTest(
            To: "user@example.com",
            UserName: "John Doe",
            Code: "123456");

        var message = JsonSerializer.Serialize(testEvent);

        // Act
        await handler.HandleAsync(message);

        // Assert
        mockEmailService.Verify(
            x => x.SendVerifyEmailAsync(It.Is<EmailVerifyDTO>(dto =>
                dto.To == "user@example.com" &&
                dto.UserName == "John Doe" &&
                dto.Code == "123456")),
            Times.Once,
            "emailService.SendVerifyEmailAsync должен быть вызван с корректными данными");
    }

    /// <summary>
    /// Проверяет, что Channel property возвращает правильное значение
    /// </summary>
    [Fact]
    public void Channel_ReturnsCorrectValue()
    {
        // Arrange
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var handler = new ConfimEmailHandler(mockScopeFactory.Object);

        // Act
        var channel = handler.Channel;

        // Assert
        Assert.Equal("email:verify", channel);
    }

    /// <summary>
    /// Проверяет обработку сообщения с пустыми значениями
    /// </summary>
    [Fact]
    public async Task HandleAsync_HandlesEmptyEmailAndUsername()
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();

        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IEmailService))).Returns(mockEmailService.Object);

        var handler = new ConfimEmailHandler(mockScopeFactory.Object);

        var testEvent = new EmailVerifyEventTest(
            To: "",
            UserName: "",
            Code: "789012");

        var message = JsonSerializer.Serialize(testEvent);

        // Act
        await handler.HandleAsync(message);

        // Assert
        mockEmailService.Verify(
            x => x.SendVerifyEmailAsync(It.Is<EmailVerifyDTO>(dto =>
                dto.To == "" &&
                dto.UserName == "" &&
                dto.Code == "789012")),
            Times.Once);
    }

    /// <summary>
    /// Проверяет, что VerificationLink всегда устанавливается в пустую строку
    /// </summary>
    [Fact]
    public async Task HandleAsync_VerificationLinkAlwaysEmpty()
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();

        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IEmailService))).Returns(mockEmailService.Object);

        var handler = new ConfimEmailHandler(mockScopeFactory.Object);

        var testEvent = new EmailVerifyEventTest(
            To: "test@test.com",
            UserName: "Test User",
            Code: "999");

        var message = JsonSerializer.Serialize(testEvent);

        // Act
        await handler.HandleAsync(message);

        // Assert
        mockEmailService.Verify(
            x => x.SendVerifyEmailAsync(It.Is<EmailVerifyDTO>(dto =>
                dto.VerificationLink == "")),
            Times.Once);
    }

    /// <summary>
    /// Проверяет, что при исключении в emailService оно пробивается наверх
    /// </summary>
    [Fact]
    public async Task HandleAsync_PropagatesExceptionFromEmailService()
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();

        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IEmailService))).Returns(mockEmailService.Object);

        var testException = new InvalidOperationException("Email service error");
        mockEmailService
            .Setup(x => x.SendVerifyEmailAsync(It.IsAny<EmailVerifyDTO>()))
            .ThrowsAsync(testException);

        var handler = new ConfimEmailHandler(mockScopeFactory.Object);

        var testEvent = new EmailVerifyEventTest(
            To: "user@example.com",
            UserName: "John Doe",
            Code: "123456");

        var message = JsonSerializer.Serialize(testEvent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(message));
        Assert.Equal("Email service error", exception.Message);
    }
}

