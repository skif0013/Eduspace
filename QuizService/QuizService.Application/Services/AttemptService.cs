using QuizService.Application.Contracts;
using QuizService.Application.Contracts.IQuizAttempt;
using QuizService.Application.Contracts.QuestionsContract;
using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.Repositories;
using QuizService.Domain.Models;

namespace QuizService.Application.Services;

public class AttemptService : IAttemptService
{
    private readonly IAttemptRepository _attemptRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQuestionScoringService _questionScoringService;
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

        var quiz = await _quizRepository.GetWithQuestionsAndOptionsByIdAsync(quizId);
        if (quiz == null) throw new Exception("Quiz not found");
        
        
        var attempt = new QuizAttempt(quizId, userId);

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
        
        var score = _questionScoringService.CalculateScore(question, request.SelectedOptionIds);
        bool isCorrect = (score >= question.MaxScore);
        
        attempt.AddAnswer(
            question.Id, 
            request.SelectedOptionIds, 
            request.TextAnswer, 
            score, 
            isCorrect
        );
        
        await _unitOfWork.SaveChangesAsync();
        
        return new SubmitAnswerResponseDTO
        {
            IsCorrect = isCorrect,
            EarnedScore = score
        };
        
    }
}