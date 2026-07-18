using QuizService.Application.Contracts;
using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Application.Repositories;
using QuizService.Domain.Models;
using BuildingBlock.UserContextMiddleware.Models;
using QuizAppService = QuizService.Application.Services.QuizService;
using QuizMapper = QuizService.Application.Services.QuizMapper;

namespace QuizService.UnitTests;

public class QuizServiceTests
{
    [Fact]
    public async Task CreateQuizAsync_AddsQuiz_SavesChanges_AndReturnsDto()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuizMapper();
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userContext = new UserContext { UserId = userId, Name = "Test User", Email = "test@example.com" };
        var service = new QuizAppService(quizRepository, unitOfWork, mapper, userContext, new FakeAttemptRepository());
        var request = new CreatingQuizRequestDTO
        {
            Name = "C# Basics",
            Description = "Intro quiz",
            PassPercentage = 75
        };

        // Act
        var result = await service.CreateQuizAsync(request);

        // Assert
        Assert.Single(quizRepository.Quizzes);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.PassPercentage, result.PassPercentage);
        Assert.False(result.IsPublished);
        Assert.False(result.IsActive);
        Assert.Equal(0, result.QuestionsCount);
    }

    [Fact]
    public async Task GetQuizByIdAsync_ReturnsMappedQuiz()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuizMapper();
        var userContext = new UserContext { UserId = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
        var quiz = TestData.CreateQuiz(name: "Test Quiz");
        quizRepository.Quizzes.Add(quiz);
        var service = new QuizAppService(quizRepository, unitOfWork, mapper, userContext, new FakeAttemptRepository());

        // Act
        var result = await service.GetQuizByIdAsync(quiz.Id);

        // Assert
        Assert.Equal(quiz.Id, result.Id);
        Assert.Equal("Test Quiz", result.Name);
    }

    [Fact]
    public async Task UpdateQuizAsync_UpdatesDomainModel_AndSavesChanges()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuizMapper();
        var userContext = new UserContext { UserId = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
        var quiz = TestData.CreateQuiz(name: "Old name", description: "Old desc", passPercentage: 50);
        quizRepository.Quizzes.Add(quiz);
        var service = new QuizAppService(quizRepository, unitOfWork, mapper, userContext, new FakeAttemptRepository());

        var request = new QuizUpdateRequestDTO
        {
            Name = "New name",
            Description = "New desc",
            Category = "Backend",
            PassPercentage = 80
        };

        // Act
        await service.UpdateQuizAsync(quiz.Id, request);

        // Assert
        Assert.Equal("New name", quiz.Name);
        Assert.Equal("New desc", quiz.Description);
        Assert.Equal(80, quiz.PassPercentage);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task DeleteQuizAsync_RemovesQuiz_AndSavesChanges()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuizMapper();
        var userContext = new UserContext { UserId = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
        var quiz = TestData.CreateQuiz(name: "To remove");
        quizRepository.Quizzes.Add(quiz);
        var service = new QuizAppService(quizRepository, unitOfWork, mapper, userContext, new FakeAttemptRepository());

        // Act
        await service.DeleteQuizAsync(quiz.Id);

        // Assert
        Assert.Empty(quizRepository.Quizzes);
        Assert.Same(quiz, quizRepository.RemovedQuiz);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task FinishQuizAsync_FinishesAttempt_AndMapsSummaryDto()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var attemptRepository = new FakeAttemptRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuizMapper();
        var userContext = new UserContext { UserId = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
        var service = new QuizAppService(quizRepository, unitOfWork, mapper, userContext, attemptRepository);

        var quiz = TestData.CreateQuiz(name: "Final");
        TestData.AttachQuestions(quiz, TestData.CreateQuestion(quiz.Id, text: "Question", options: [("Correct", true, 10, 1)]));
        var attempt = TestData.CreateAttempt(quiz.Id);
        attempt.Quiz = quiz;
        attempt.TotalScore = 10;
        attemptRepository.Attempts.Add(attempt);

        // Act
        var result = await service.FinishQuizAsync(attempt.Id);

        // Assert
        Assert.Equal(attempt.Id, result.AttemptId);
        Assert.Equal(10, result.TotalScore);
        Assert.Equal(1, result.TotalQuestions);
        Assert.True(result.IsPassed);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
        Assert.Equal(QuizService.Domain.Enum.AttemptStatus.Completed, attempt.Status);
        Assert.NotNull(attempt.FinishedAt);
    }
}
