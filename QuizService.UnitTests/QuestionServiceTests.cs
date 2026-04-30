using QuizService.Application.DTOs.QuestionsDTOs.CreateRequestDTOs;
using QuizService.Domain.Enum;
using QuestionAppService = QuizService.Application.Services.QuestionService;
using QuestionMapper = QuizService.Application.Services.QuestionMapper;

namespace QuizService.UnitTests;

public class QuestionServiceTests
{
    [Fact]
    public async Task CreateQuestionWithTitleAsync_CreatesQuestion_WithOptions_AndSaves()
    {
        // Arrange
        var questionRepository = new FakeQuestionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuestionMapper();
        var service = new QuestionAppService(unitOfWork, questionRepository, mapper);
        var quizId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var request = new CreateQuestionRequestDTO
        {
            QuizId = quizId,
            Text = "What is C#?",
            Order = 1,
            MaxScore = 10,
            QuestionType = QuestionType.SingleChoice,
            Options =
            [
                new CreateAnswerOptionInsideQuestionDTO { Text = "A language", IsCorrect = true, Score = 10, Order = 1 },
                new CreateAnswerOptionInsideQuestionDTO { Text = "A database", IsCorrect = false, Score = 0, Order = 2 }
            ]
        };

        // Act
        var result = await service.CreateQuestionWithTitleAsync(request);

        // Assert
        Assert.Single(questionRepository.Questions);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
        Assert.Equal(request.Text, result.Text);
        Assert.Equal(request.QuizId, result.QuizId);
        Assert.Equal(2, result.Options.Count);
        Assert.Equal("A language", result.Options[0].Text);
    }

    [Fact]
    public async Task GetQuestionByIdAsync_ReturnsMappedQuestion()
    {
        // Arrange
        var questionRepository = new FakeQuestionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuestionMapper();
        var service = new QuestionAppService(unitOfWork, questionRepository, mapper);
        var question = TestData.CreateQuestion(Guid.NewGuid(), text: "Question text", options: [("Correct", true, 10, 1)]);
        questionRepository.Questions.Add(question);

        // Act
        var result = await service.GetQuestionByIdAsync(question.Id);

        // Assert
        Assert.Equal(question.Id, result.QuestionId);
        Assert.Equal(question.QuizId, result.QuizId);
        Assert.Equal(question.Text, result.Text);
        Assert.Single(result.Options);
    }

    [Fact]
    public async Task GetQuestionByIdAsync_WhenQuestionMissing_Throws()
    {
        // Arrange
        var questionRepository = new FakeQuestionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuestionMapper();
        var service = new QuestionAppService(unitOfWork, questionRepository, mapper);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => service.GetQuestionByIdAsync(Guid.NewGuid()));
        Assert.Equal("Question not found", ex.Message);
    }

    [Fact]
    public async Task GetAllQuestionsAsync_ReturnsMappedCollection()
    {
        // Arrange
        var questionRepository = new FakeQuestionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuestionMapper();
        var service = new QuestionAppService(unitOfWork, questionRepository, mapper);
        questionRepository.Questions.AddRange(
        [
            TestData.CreateQuestion(Guid.NewGuid(), text: "Q1", order: 1, options: [("A", true, 10, 1)]),
            TestData.CreateQuestion(Guid.NewGuid(), text: "Q2", order: 2, options: [("B", true, 10, 1)])
        ]);

        // Act
        var result = (await service.GetAllQuestionsAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, q => q.Text == "Q1");
        Assert.Contains(result, q => q.Text == "Q2");
    }

    [Fact]
    public async Task UpdateQuestionToQuizAsync_UpdatesQuestion_AndReturnsMappedDto()
    {
        // Arrange
        var questionRepository = new FakeQuestionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuestionMapper();
        var service = new QuestionAppService(unitOfWork, questionRepository, mapper);
        var question = TestData.CreateQuestion(Guid.NewGuid(), text: "Old text", order: 1, maxScore: 5, questionType: QuestionType.TrueFalse);
        question.IsActive = false;
        questionRepository.Questions.Add(question);
        var request = new CreateQuestionRequestDTO
        {
            QuizId = Guid.NewGuid(),
            Text = "New text",
            Order = 3,
            MaxScore = 15,
            QuestionType = QuestionType.MultipleChoice
        };

        // Act
        var result = await service.UpdateQuestionToQuizAsync(request, question.Id);

        // Assert
        Assert.Equal(request.Text, question.Text);
        Assert.Equal(request.Order, question.Order);
        Assert.Equal(request.MaxScore, question.MaxScore);
        Assert.Equal(request.QuestionType, question.QuestionType);
        Assert.True(question.IsActive);
        Assert.Equal(request.QuizId, question.QuizId);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
        Assert.Equal(request.Text, result.Text);
    }

    [Fact]
    public async Task CompletedQuestionAsync_MarksQuestionInactive_AndSaves()
    {
        // Arrange
        var questionRepository = new FakeQuestionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuestionMapper();
        var service = new QuestionAppService(unitOfWork, questionRepository, mapper);
        var question = TestData.CreateQuestion(Guid.NewGuid(), text: "Active question");
        questionRepository.Questions.Add(question);

        // Act
        var result = await service.CompletedQuestionAsync(question.Id);

        // Assert
        Assert.False(question.IsActive);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task DeleteQuestionFromQuizAsync_RemovesQuestion_AndSaves()
    {
        // Arrange
        var questionRepository = new FakeQuestionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var mapper = new QuestionMapper();
        var service = new QuestionAppService(unitOfWork, questionRepository, mapper);
        var question = TestData.CreateQuestion(Guid.NewGuid(), text: "Delete me");
        questionRepository.Questions.Add(question);

        // Act
        var result = await service.DeleteQuestionFromQuizAsync(question.Id);

        // Assert
        Assert.True(result);
        Assert.Empty(questionRepository.Questions);
        Assert.Same(question, questionRepository.RemovedQuestion);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }
}

