using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.Services;
using QuizService.Domain.Models;
using AttemptAppService = QuizService.Application.Services.AttemptService;
using QuestionMapper = QuizService.Application.Services.QuestionMapper;
using QuizMapper = QuizService.Application.Services.QuizMapper;

namespace QuizService.UnitTests;

public class AttemptServiceTests
{
    [Fact]
    public async Task StartQuizAsync_CreatesAttempt_AndReturnsOrderedQuestions()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var questionRepository = new FakeQuestionRepository();
        var attemptRepository = new FakeAttemptRepository
        {
            HasActiveAttemptHandler = (_, _) => Task.FromResult(false)
        };
        var unitOfWork = new FakeUnitOfWork();
        var scoringService = new QuestionScoringService();
        var mapper = new QuizMapper();
        var service = new AttemptAppService(attemptRepository, questionRepository, unitOfWork, scoringService, mapper, quizRepository);

        var quiz = TestData.CreateQuiz(name: "Attempt quiz");
        var q2 = TestData.CreateQuestion(quiz.Id, text: "Second", order: 2, options: [("B", true, 10, 1)]);
        var q1 = TestData.CreateQuestion(quiz.Id, text: "First", order: 1, options: [("A", true, 10, 1)]);
        TestData.AttachQuestions(quiz, q2, q1);
        quizRepository.GetWithQuestionsAndOptionsHandler = _ => Task.FromResult<Quiz?>(quiz);

        // Act
        var result = await service.StartQuizAsync(quiz.Id, Guid.Parse("33333333-3333-3333-3333-333333333333"));

        // Assert
        Assert.Single(attemptRepository.Attempts);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
        Assert.Equal(2, result.TotalQuestions);
        Assert.Equal("First", result.Questions[0].Text);
        Assert.Equal(1, result.Questions[0].Order);
        Assert.Equal("Second", result.Questions[1].Text);
        Assert.Equal(2, result.Questions[1].Order);
    }

    [Fact]
    public async Task StartQuizAsync_WhenActiveAttemptExists_Throws()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var questionRepository = new FakeQuestionRepository();
        var attemptRepository = new FakeAttemptRepository
        {
            HasActiveAttemptHandler = (_, _) => Task.FromResult(true)
        };
        var unitOfWork = new FakeUnitOfWork();
        var scoringService = new QuestionScoringService();
        var mapper = new QuizMapper();
        var service = new AttemptAppService(attemptRepository, questionRepository, unitOfWork, scoringService, mapper, quizRepository);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => service.StartQuizAsync(Guid.NewGuid(), Guid.NewGuid()));
        Assert.Equal("User already has an active attempt for this quiz", ex.Message);
    }

    [Fact]
    public async Task SubmitAnswerAsync_CalculatesScore_AndStoresAnswer()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var questionRepository = new FakeQuestionRepository();
        var attemptRepository = new FakeAttemptRepository();
        var unitOfWork = new FakeUnitOfWork();
        var scoringService = new QuestionScoringService();
        var mapper = new QuizMapper();
        var service = new AttemptAppService(attemptRepository, questionRepository, unitOfWork, scoringService, mapper, quizRepository);

        var quiz = TestData.CreateQuiz(name: "Quiz");
        var question = TestData.CreateQuestion(quiz.Id, text: "Choose one", maxScore: 10, options:
        [
            ("Wrong", false, 0, 1),
            ("Right", true, 10, 2)
        ]);
        questionRepository.Questions.Add(question);
        var attempt = TestData.CreateAttempt(quiz.Id);
        attemptRepository.Attempts.Add(attempt);
        questionRepository.GetWithOptionsByIdHandler = _ => Task.FromResult(question);
        attemptRepository.GetByIdHandler = _ => Task.FromResult<QuizAttempt?>(attempt);

        var request = new SubmitAnswerRequestDTO
        {
            QuestionId = question.Id,
            SelectedOptionIds = [question.AnswerOptions.First(o => o.IsCorrectAnswer).Id],
            TextAnswer = null
        };

        // Act
        var result = await service.SubmitAnswerAsync(attempt.Id, request);

        // Assert
        Assert.True(result.IsCorrect);
        Assert.Equal(10, result.EarnedScore);
        Assert.Equal(10, attempt.TotalScore);
        Assert.Single(attempt.Answers);
        Assert.True(attempt.Answers.Single().IsCorrect);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task SubmitAnswerAsync_WhenAttemptMissing_Throws()
    {
        // Arrange
        var quizRepository = new FakeQuizRepository();
        var questionRepository = new FakeQuestionRepository();
        var attemptRepository = new FakeAttemptRepository();
        var unitOfWork = new FakeUnitOfWork();
        var scoringService = new QuestionScoringService();
        var mapper = new QuizMapper();
        var service = new AttemptAppService(attemptRepository, questionRepository, unitOfWork, scoringService, mapper, quizRepository);

        questionRepository.GetWithOptionsByIdHandler = _ => Task.FromResult(TestData.CreateQuestion(Guid.NewGuid()));
        attemptRepository.GetByIdHandler = _ => Task.FromResult<QuizAttempt?>(null);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => service.SubmitAnswerAsync(Guid.NewGuid(), new SubmitAnswerRequestDTO { QuestionId = Guid.NewGuid(), SelectedOptionIds = [] }));
        Assert.Equal("Attempt not found", ex.Message);
    }
}

