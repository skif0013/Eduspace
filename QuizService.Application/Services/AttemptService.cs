using QuizService.Application.Contracts;
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
    private readonly IQuestionRepository _questionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQuestionScoringService _questionScoringService;
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IQuizMapper _quizMapper;
    private readonly IQuizRepository _quizRepository;
    
    
    public AttemptService(IAttemptRepository attemptRepository,IQuestionRepository questionRepository, IUnitOfWork unitOfWork, IQuestionScoringService questionScoringService,IQuizMapper quizMapper, IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
        _quizMapper = quizMapper;
        _attemptRepository = attemptRepository;
        _questionRepository = questionRepository;
        _unitOfWork = unitOfWork;
        _questionScoringService = questionScoringService;
    }
    
    public async Task<QuizStartResponseDTO> StartQuizAsync(Guid quizId, Guid userId)
    {
        if (await _attemptRepository.HasActiveAttemptAsync(quizId, userId))
            throw new Exception("User already has an active attempt for this quiz");

        var quiz = await  _quizRepository.GetWithQuestionsAndOptionsByIdAsync(quizId);
        if (quiz == null)
            throw new Exception("Quiz not found");
        
        
        var attempt = new QuizAttempt
        {
            Id = Guid.NewGuid(),
            QuizId = quizId,
            UserId = userId,
            Status = AttemptStatus.InProgress,
            TotalScore = 0,
            StartedAt = DateTime.UtcNow
        };

        await _attemptRepository.AddAsync(attempt);
        await _unitOfWork.SaveChangesAsync();

        return _quizMapper.ToStartResponseDTO(attempt, quiz.Questions);
    }

    public async Task<SubmitAnswerResponseDTO> SubmitAnswerAsync(Guid attemptId, SubmitAnswerRequestDTO request)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId);
        
        if (attempt == null)
            throw new Exception("Attempt not found");

        var question = await _questionRepository.GetWithOptionsByIdAsync(request.QuestionId);
        
        if (question == null)
            throw new Exception("Question not found");

        double ernedScore = _questionScoringService.CalculateScore(question, request.SelectedOptionIds);
        bool isCorrect = ernedScore == question.MaxScore;

        var userAnswer = new UserAnswer()
        {
            Id = Guid.NewGuid(),
            QuestionId = request.QuestionId,
            SelectedOptionId = request.SelectedOptionIds,
            IsCorrect = isCorrect,
            TextAnswer = request.TextAnswer,
            AttemptId = attemptId
        };
        
        await _userAnswerRepository.AddAsync(userAnswer);
        
        attempt.TotalScore += ernedScore;

        await _unitOfWork.SaveChangesAsync();
        
        return new SubmitAnswerResponseDTO
        {
            IsCorrect = isCorrect,
            EarnedScore = ernedScore,
        };
    }


    public async Task<FinishQuizResponseDTO> FinishQuizAsync(Guid quizId)
    {
        var attempt = await _attemptRepository.GetWithAnswersAsync(quizId);
        if (attempt == null)
            throw new Exception("Attempt not found");

        if (attempt.Status == AttemptStatus.Completed)
        {
            throw new Exception("Quiz is already completed");
        }
        
        attempt.Status = AttemptStatus.Completed;
        attempt.FinishedAt = DateTime.UtcNow;
        
        await _unitOfWork.SaveChangesAsync();
        
        return _quizMapper.MapToFinishQuizResponseDTO(attempt);
    }
}