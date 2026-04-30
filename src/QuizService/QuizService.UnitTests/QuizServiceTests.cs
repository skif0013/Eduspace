using QuizService.Application.Contracts;
using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Application.Repositories;
using QuizService.Domain.Models;
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
        var service = new QuizAppService(quizRepository, unitOfWork, mapper, new NoOpTokenService(), new FakeAttemptRepository());
        var request = new CreatingQuizRequestDTO
        {
            Name = "C# Basics",
            Description = "Intro quiz",
            PassPercentage = 75
        };

        // Act
        var result = await service.CreateQuizAsync(request, Guid.Parse("11111111-1111-1111-1111-111111111111"));

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
    public async Task GetAllQuizzesAsync_ReturnsMappedQuizzes()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuizMapper();
        var quiz1 = TestData.CreateQuiz(name: "Quiz 1");
        var quiz2 = TestData.CreateQuiz(name: "Quiz 2");
        quizRepository.Quizzes.AddRange([quiz1, quiz2]);
        var service = new QuizAppService(quizRepository, unitOfWork, mapper, new NoOpTokenService(), new FakeAttemptRepository());

        // Act
        var result = (await service.GetAllQuizzesAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, q => q.Name == "Quiz 1");
        Assert.Contains(result, q => q.Name == "Quiz 2");
    }

    [Fact]
    public async Task UpdateQuizAsync_UpdatesDomainModel_AndSavesChanges()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuizMapper();
        var quiz = TestData.CreateQuiz(name: "Old name", description: "Old desc", passPercentage: 50);
        quizRepository.Quizzes.Add(quiz);
        var service = new QuizAppService(quizRepository, unitOfWork, mapper, new NoOpTokenService(), new FakeAttemptRepository());

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
        var quiz = TestData.CreateQuiz(name: "To remove");
        quizRepository.Quizzes.Add(quiz);
        var service = new QuizAppService(quizRepository, unitOfWork, mapper, new NoOpTokenService(), new FakeAttemptRepository());

        // Act
        await service.DeleteQuizAsync(quiz.Id);

        // Assert
        Assert.Empty(quizRepository.Quizzes);
        Assert.Same(quiz, quizRepository.RemovedQuiz);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task PublishQuizAsync_PublishesQuizWithQuestions_AndReturnsDto()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuizMapper();
        var quiz = TestData.CreateQuiz(name: "Publish me");
        var question = TestData.CreateQuestion(quiz.Id, order: 2, text: "Q1", options: [("A", true, 10, 1)]);
        TestData.AttachQuestions(quiz, question);
        quizRepository.Quizzes.Add(quiz);
        quizRepository.GetWithQuestionsAndOptionsHandler = _ => Task.FromResult<Quiz?>(quiz);
        var service = new QuizAppService(quizRepository, unitOfWork, mapper, new NoOpTokenService(), new FakeAttemptRepository());

        // Act
        var result = await service.PublishQuizAsync(quiz.Id);

        // Assert
        Assert.True(quiz.IsPublished);
        Assert.True(quiz.IsActive);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
        Assert.Equal(1, result.QuestionsCount);
        Assert.Single(result.Questions!);
        Assert.Equal("Q1", result.Questions![0].Text);
    }

    [Fact]
    public async Task FinishQuizAsync_FinishesAttempt_AndMapsSummaryDto()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var attemptRepository = new FakeAttemptRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuizMapper();
        var service = new QuizAppService(quizRepository, unitOfWork, mapper, new NoOpTokenService(), attemptRepository);

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

