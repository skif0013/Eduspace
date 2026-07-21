using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using NotificationService.Application.Redis.EventHadnlers;

namespace NotificationService.UnitTests;

// Тестовая модель события. Добавлено поле VerificationLink для совпадения со структурой реального EmailVerifyEvent
public record EmailVerifyEventTest(string To, string UserName, string Code, string VerificationLink = "");

/// <summary>
/// Модульные тесты для ConfirmEmailHandler
/// Проверяют корректность обработки сообщений верификации email
/// </summary>
public class ConfirmEmailHandlerTests
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

        mockScopeFactory
            .Setup(x => x.CreateScope())
            .Returns(mockServiceScope.Object);

        mockServiceScope
            .Setup(x => x.ServiceProvider)
            .Returns(mockServiceProvider.Object);

        mockServiceProvider
            .Setup(x => x.GetService(typeof(IEmailService)))
            .Returns(mockEmailService.Object);

        var handler = new ConfirmEmailHandler(mockScopeFactory.Object);

        var testEvent = new EmailVerifyEventTest(
            To: "user@example.com",
            UserName: "John Doe",
            Code: "123456",
            VerificationLink: "https://example.com/verify");

        var message = JsonSerializer.Serialize(testEvent);

        // Act
        await handler.HandleMessageAsync(message);

        // Assert
        mockEmailService.Verify(
            x => x.SendVerifyEmailAsync(It.Is<EmailVerifyDTO>(dto =>
                dto.To == "user@example.com" &&
                dto.UserName == "John Doe" &&
                dto.Code == "123456" &&
                dto.VerificationLink == "https://example.com/verify")),
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
        var handler = new ConfirmEmailHandler(mockScopeFactory.Object);

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

        var handler = new ConfirmEmailHandler(mockScopeFactory.Object);

        var testEvent = new EmailVerifyEventTest(
            To: "",
            UserName: "",
            Code: "789012");

        var message = JsonSerializer.Serialize(testEvent);

        // Act
        await handler.HandleMessageAsync(message);

        // Assert
        mockEmailService.Verify(
            x => x.SendVerifyEmailAsync(It.Is<EmailVerifyDTO>(dto =>
                dto.To == "" &&
                dto.UserName == "" &&
                dto.Code == "789012")),
            Times.Once);
    }

    /// <summary>
    /// Проверяет, что VerificationLink правильно передается как пустая строка
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

        var handler = new ConfirmEmailHandler(mockScopeFactory.Object);

        var testEvent = new EmailVerifyEventTest(
            To: "test@test.com",
            UserName: "Test User",
            Code: "999",
            VerificationLink: ""); // Явно передаем пустую строку

        var message = JsonSerializer.Serialize(testEvent);

        // Act
        await handler.HandleMessageAsync(message);

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

        var handler = new ConfirmEmailHandler(mockScopeFactory.Object);

        var testEvent = new EmailVerifyEventTest(
            To: "user@example.com",
            UserName: "John Doe",
            Code: "123456");

        var message = JsonSerializer.Serialize(testEvent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleMessageAsync(message));
        Assert.Equal("Email service error", exception.Message);
    }
}