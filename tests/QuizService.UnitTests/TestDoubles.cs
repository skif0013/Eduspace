using System.Reflection;
using QuizService.Application.Contracts;
using QuizService.Application.Contracts.IQuizAttempt;
using QuizService.Application.Contracts.QuestionsContract;
using QuizService.Application.DTOs;
using QuizService.Application.Repositories;
using QuizService.Domain.Enum;
using QuizService.Domain.Models;

namespace QuizService.UnitTests;

public static class TestData
{
    public static Quiz CreateQuiz(
        Guid? creatorId = null,
        string name = "Quiz",
        string? description = "Description",
        double passPercentage = 50)
        => new(creatorId ?? Guid.NewGuid(), name, description, passPercentage);

    public static Question CreateQuestion(
        Guid quizId,
        string text = "Question",
        int order = 1,
        int maxScore = 10,
        QuestionType questionType = QuestionType.SingleChoice,
        params (string Text, bool IsCorrect, double Score, int Order)[] options)
    {
        var question = new Question(quizId, text, order, maxScore, questionType);

        foreach (var option in options)
        {
            question.AddAnswerOption(option.Text, option.IsCorrect, option.Score, option.Order);
        }

        return question;
    }

    public static QuizAttempt CreateAttempt(Guid quizId, Guid? userId = null)
        => new(quizId, userId ?? Guid.NewGuid());

    public static void AttachQuestions(Quiz quiz, params Question[] questions)
    {
        var field = typeof(Quiz).GetField("_questions", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?? throw new InvalidOperationException("Quiz questions field not found.");

        var list = (List<Question>)field.GetValue(quiz)!;
        list.AddRange(questions);
    }

    public static void AttachAnswers(QuizAttempt attempt, params UserAnswer[] answers)
    {
        var field = typeof(QuizAttempt).GetField("_answers", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?? throw new InvalidOperationException("QuizAttempt answers field not found.");

        var list = (List<UserAnswer>)field.GetValue(attempt)!;
        list.AddRange(answers);
    }
}

public sealed class FakeQuizRepository : IQuizRepository
{
    public List<Quiz> Quizzes { get; } = new();
    public Func<Task<IReadOnlyCollection<Quiz>>>? GetAllHandler { get; set; }
    public Func<Quiz, Task>? AddHandler { get; set; }
    public Func<Guid, Task<Quiz?>>? FindByIdHandler { get; set; }
    public Func<Quiz, Task>? RemoveHandler { get; set; }
    public Func<Guid, Task<Quiz?>>? GetWithQuestionsAndOptionsHandler { get; set; }

    public Quiz? RemovedQuiz { get; private set; }

    public Task<IReadOnlyCollection<Quiz>> GetAllQuizzesAsync()
        => GetAllHandler?.Invoke() ?? Task.FromResult((IReadOnlyCollection<Quiz>)Quizzes.ToList());

    public async Task AddQuizAsync(Quiz quiz)
    {
        Quizzes.Add(quiz);
        if (AddHandler != null)
        {
            await AddHandler(quiz);
        }
    }

    public Task<Quiz?> FindByIdAsync(Guid quizId)
        => FindByIdHandler?.Invoke(quizId) ?? Task.FromResult(Quizzes.FirstOrDefault(q => q.Id == quizId));

    public async Task RemoveAsync(Quiz quiz)
    {
        RemovedQuiz = quiz;
        Quizzes.Remove(quiz);
        if (RemoveHandler != null)
        {
            await RemoveHandler(quiz);
        }
    }

    public Task<Quiz?> GetWithQuestionsAndOptionsByIdAsync(Guid quizId)
        => GetWithQuestionsAndOptionsHandler?.Invoke(quizId)
           ?? Task.FromResult(Quizzes.FirstOrDefault(q => q.Id == quizId));
}

public sealed class FakeQuestionRepository : IQuestionRepository
{
    public List<Question> Questions { get; } = new();
    public Func<Task<IReadOnlyCollection<Question>>>? GetAllHandler { get; set; }
    public Func<Question, Task>? AddHandler { get; set; }
    public Func<Guid, Task<Question?>>? FindByIdHandler { get; set; }
    public Func<Question, Task>? RemoveHandler { get; set; }
    public Func<Guid, Task<List<Question>>>? GetActiveByQuizIdHandler { get; set; }
    public Func<Guid, Task<Question>>? GetWithOptionsByIdHandler { get; set; }

    public Question? RemovedQuestion { get; private set; }

    public Task<IReadOnlyCollection<Question>> GetAllQuestionsAsync()
        => GetAllHandler?.Invoke() ?? Task.FromResult((IReadOnlyCollection<Question>)Questions.ToList());

    public async Task AddQuestion(Question question)
    {
        Questions.Add(question);
        if (AddHandler != null)
        {
            await AddHandler(question);
        }
    }

    public Task<Question?> FindByIdAsync(Guid questionId)
        => FindByIdHandler?.Invoke(questionId) ?? Task.FromResult(Questions.FirstOrDefault(q => q.Id == questionId));

    public async Task RemoveAsync(Question question)
    {
        RemovedQuestion = question;
        Questions.Remove(question);
        if (RemoveHandler != null)
        {
            await RemoveHandler(question);
        }
    }

    public Task<List<Question>> GetActiveByQuizIdAsync(Guid quizId)
        => GetActiveByQuizIdHandler?.Invoke(quizId)
           ?? Task.FromResult(Questions.Where(q => q.QuizId == quizId && q.IsActive).ToList());

    public Task<Question> GetWithOptionsByIdAsync(Guid questionId)
        => GetWithOptionsByIdHandler?.Invoke(questionId)
           ?? Task.FromResult(Questions.First(q => q.Id == questionId));
}

public sealed class FakeAttemptRepository : IAttemptRepository
{
    public List<QuizAttempt> Attempts { get; } = new();
    public Func<QuizAttempt, Task>? AddHandler { get; set; }
    public Func<Guid, Task<QuizAttempt?>>? GetByIdHandler { get; set; }
    public Func<Guid, Task<QuizAttempt?>>? GetWithAnswersHandler { get; set; }
    public Func<Guid, Guid, Task<bool>>? HasActiveAttemptHandler { get; set; }

    public QuizAttempt? AddedAttempt { get; private set; }

    public async Task AddAsync(QuizAttempt attempt)
    {
        AddedAttempt = attempt;
        Attempts.Add(attempt);
        if (AddHandler != null)
        {
            await AddHandler(attempt);
        }
    }

    public Task<QuizAttempt?> GetByIdAsync(Guid attemptId)
        => GetByIdHandler?.Invoke(attemptId) ?? Task.FromResult(Attempts.FirstOrDefault(a => a.Id == attemptId));

    public Task<QuizAttempt?> GetWithAnswersAsync(Guid attemptId)
        => GetWithAnswersHandler?.Invoke(attemptId) ?? Task.FromResult(Attempts.FirstOrDefault(a => a.Id == attemptId));

    public Task<bool> HasActiveAttemptAsync(Guid quizId, Guid userId)
        => HasActiveAttemptHandler?.Invoke(quizId, userId)
           ?? Task.FromResult(Attempts.Any(a => a.QuizId == quizId && a.UserId == userId && a.Status == AttemptStatus.InProgress));
}

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCallCount { get; private set; }

    public Task<int> SaveChangesAsync()
    {
        SaveChangesCallCount++;
        return Task.FromResult(1);
    }
}

public sealed class NoOpTokenService : ITokenService
{
    public QuizService.Application.DTOs.UserContextDTO GetUserFromToken(string token) => new();

    public Guid GetUserIdFromToken(string token) => Guid.Empty;
    
    public string GetUserEmailFromToken(string token) => "test@example.com";
}

public sealed class NoOpEventPublisher : IQuizFinishedEventPublisher
{
    public Task PublishAsync(QuizFinishedEvent @event, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
