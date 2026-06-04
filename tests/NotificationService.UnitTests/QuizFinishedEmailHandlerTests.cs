using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using NotificationService.Infrastructure.Service;
using BuildingBlocks.Redis.Events;

namespace NotificationService.UnitTests;

/// <summary>
/// Модульные тесты для QuizFinishedEmailHandler
/// Проверяют корректность отправки email при завершении квиза
/// </summary>
public class QuizFinishedEmailHandlerTests
{
    /// <summary>
    /// Проверяет, что handler десериализует событие и отправляет email при прохождении квиза
    /// </summary>
    [Fact]
    public async Task HandleAsync_SendsEmailWithPassedStatusWhenQuizPassed()
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();

        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IEmailService))).Returns(mockEmailService.Object);

        var handler = new QuizFinishedEmailHandler(mockScopeFactory.Object);

        var quizEvent = new QuizFinishedEvent(
            AttemptId: Guid.NewGuid(),
            UserEmail: "user@example.com",
            TotalScore: "95",
            IsPassed: true);

        var message = JsonSerializer.Serialize(quizEvent);

        // Act
        await handler.HandleAsync(message);

        // Assert
        mockEmailService.Verify(
            x => x.SendEmailAsync(It.Is<EmailSendDTO>(dto =>
                dto.To == "user@example.com" &&
                dto.Subject.Contains("Successfully") &&
                dto.Body.Contains("PASSED") &&
                dto.Body.Contains("95"))),
            Times.Once);
    }

    /// <summary>
    /// Проверяет, что handler отправляет email при неудачном прохождении квиза
    /// </summary>
    [Fact]
    public async Task HandleAsync_SendsEmailWithFailedStatusWhenQuizFailed()
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();

        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IEmailService))).Returns(mockEmailService.Object);

        var handler = new QuizFinishedEmailHandler(mockScopeFactory.Object);

        var quizEvent = new QuizFinishedEvent(
            AttemptId: Guid.NewGuid(),
            UserEmail: "user@example.com",
            TotalScore: "45",
            IsPassed: false);

        var message = JsonSerializer.Serialize(quizEvent);

        // Act
        await handler.HandleAsync(message);

        // Assert
        mockEmailService.Verify(
            x => x.SendEmailAsync(It.Is<EmailSendDTO>(dto =>
                dto.To == "user@example.com" &&
                dto.Subject.Contains("Review Results") &&
                dto.Body.Contains("FAILED") &&
                dto.Body.Contains("45"))),
            Times.Once);
    }

    /// <summary>
    /// Проверяет, что Channel property возвращает правильный Redis stream ключ
    /// </summary>
    /*[Fact]
    public void Channel_ReturnsCorrectStreamKey()
    {
        // Arrange
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var handler = new QuizFinishedEmailHandler(mockScopeFactory.Object);

        // Act
        var channel = handler.Channel;

        // Assert
        Assert.Equal("quiz:finished:v1", channel);
    }*/

    /// <summary>
    /// Проверяет обработку события с различными баллами
    /// </summary>
    [Theory]
    [InlineData("100", true)]
    [InlineData("50", true)]
    [InlineData("0", false)]
    [InlineData("75", true)]
    public async Task HandleAsync_HandlesVariousScores(string score, bool isPassed)
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();

        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IEmailService))).Returns(mockEmailService.Object);

        var handler = new QuizFinishedEmailHandler(mockScopeFactory.Object);

        var quizEvent = new QuizFinishedEvent(
            AttemptId: Guid.NewGuid(),
            UserEmail: "test@test.com",
            TotalScore: score,
            IsPassed: isPassed);

        var message = JsonSerializer.Serialize(quizEvent);

        // Act
        await handler.HandleAsync(message);

        // Assert
        mockEmailService.Verify(
            x => x.SendEmailAsync(It.Is<EmailSendDTO>(dto =>
                dto.Body.Contains(score))),
            Times.Once);
    }

    /// <summary>
    /// Проверяет, что при невалидном JSON обработчик не падает
    /// </summary>
    [Fact]
    public async Task HandleAsync_HandlesInvalidJsonGracefully()
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();

        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IEmailService))).Returns(mockEmailService.Object);

        var handler = new QuizFinishedEmailHandler(mockScopeFactory.Object);
        var invalidJson = "{ invalid json }";

        // Act & Assert
        // Ожидаем исключение при десериализации
        await Assert.ThrowsAsync<JsonException>(() => handler.HandleAsync(invalidJson));

        // emailService не должен быть вызван
        mockEmailService.Verify(
            x => x.SendEmailAsync(It.IsAny<EmailSendDTO>()),
            Times.Never);
    }

    /// <summary>
    /// Проверяет, что при исключении в emailService оно пробивается наверх
    /// </summary>
    [Fact]
    public async Task HandleAsync_PropagatesExceptionFromEmailService()
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        var mockServiceScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();

        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockServiceScope.Object);
        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockServiceProvider.Setup(x => x.GetService(typeof(IEmailService))).Returns(mockEmailService.Object);

        var testException = new InvalidOperationException("SMTP connection failed");
        mockEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<EmailSendDTO>()))
            .ThrowsAsync(testException);

        var handler = new QuizFinishedEmailHandler(mockScopeFactory.Object);

        var quizEvent = new QuizFinishedEvent(
            AttemptId: Guid.NewGuid(),
            UserEmail: "user@example.com",
            TotalScore: "90",
            IsPassed: true);

        var message = JsonSerializer.Serialize(quizEvent);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(message));
        Assert.Equal("SMTP connection failed", exception.Message);
    }
}

