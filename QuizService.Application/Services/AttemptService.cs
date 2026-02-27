using QuizService.Application.Contracts.IQuizAttempt;
using QuizService.Application.Contracts.QuestionsContract;
using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.Repositories;
using QuizService.Domain.Enum;
using QuizService.Domain.Models;

namespace QuizService.Application.Services;

public class AttemptService : IAttemptService
{
    private readonly IAttemptRepository _attemptRepository;
    private readonly IQuizRepository _quizRepository; 
    private readonly IQuestionRepository _questionRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public AttemptService(IAttemptRepository attemptRepository, IQuizRepository quizRepository, IQuestionRepository questionRepository, IUnitOfWork unitOfWork)
    {
        _attemptRepository = attemptRepository;
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<QuizStartResponseDTO> StartQuizAsync(Guid quizId, Guid userId)
    {
        var hasActive = await _attemptRepository.HasActiveAttemptAsync(quizId, userId);
        if (hasActive)
            throw new Exception("You already have active attempt");

        var quiz = await _quizRepository.FindByIdAsync(quizId);
        if (quiz == null)
            throw new Exception("Quiz not found");

        var questions = await _questionRepository.GetActiveByQuizIdAsync(quizId);

        var attempt = new QuizAttempt
        {
            Id = Guid.NewGuid(),
            QuizId = quizId,
            UserId = userId,
            StartedAt = DateTime.UtcNow,
            Status = AttemptStatus.InProgress
        };

        await _attemptRepository.AddAsync(attempt);
        await _unitOfWork.SaveChangesAsync();

        return new QuizStartResponseDTO
        {
            AttemptId = attempt.Id,
            StartedAt = attempt.StartedAt,
            TotalQuestions = questions.Count,
            Questions = questions.Select(q => new QuestionForAttemptDTO
            {
                QuestionId = q.Id,
                Text = q.Text,
                QuestionType = q.QuestionType,
                Order = q.Order,
                Options = q.AnswerOptions.Select(o => new OptionForAttemptDTO
                {
                    OptionId = o.Id,
                    Text = o.Text,
                    Order = o.Order
                }).ToList()
            }).ToList()
        };
    }
    
    public async Task<SubmitAnswerResponseDTO> SubmitAnswerAsync(Guid attemptId, SubmitAnswerRequestDTO request)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId);
        if (attempt == null)
            throw new Exception("Attempt not found");

        if (attempt.Status != AttemptStatus.InProgress)
            throw new Exception("Quiz already finished");

        var question = await _questionRepository.GetWithOptionsAsync(request.QuestionId);
        if (question == null)
            throw new Exception("Question not found");

        double earnedScore = 0;
        bool isCorrect = false;

        var correctOptions = question.Options.Where(o => o.IsCorrect).ToList();

        if (question.QuestionType == QuestionType.SingleChoice)
        {
            var correctId = correctOptions.First().Id;
            isCorrect = request.SelectedOptionIds?.Contains(correctId) == true;
            earnedScore = isCorrect ? correctOptions.First().Score : 0;
        }

        var answer = new UserAnswer
        {
            Id = Guid.NewGuid(),
            AttemptId = attemptId,
            QuestionId = question.Id,
            IsCorrect = isCorrect,
            Score = earnedScore
        };

        await _userAnswerRepository.AddAsync(answer);
        await _unitOfWork.SaveChangesAsync();

        return new SubmitAnswerResponseDTO
        {
            IsCorrect = isCorrect,
            EarnedScore = earnedScore
        };
    }
}