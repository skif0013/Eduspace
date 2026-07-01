using BuildingBlocks.Redis.Contracts.Broker;
using QuizService.Application.Contracts;
using QuizService.Application.Contracts.IQuizAttempt;
using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Application.Exceptions;
using QuizService.Application.Repositories;
using QuizService.Domain.Models;

namespace QuizService.Application.Services;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQuizMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IAttemptRepository _attemptRepository;
    private readonly IRedisMessageBroker _eventPublisher;
    private readonly IQuizIntegrationEventService _quizIntegrationEventService;

    public QuizService(IQuizRepository quizRepository, IUnitOfWork unitOfWork, IQuizMapper mapper,
        ITokenService tokenService, IAttemptRepository attemptRepository, IRedisMessageBroker eventPublisher
        , IQuizIntegrationEventService quizIntegrationEventService)
    {
        _quizIntegrationEventService = quizIntegrationEventService;
        _quizRepository = quizRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tokenService = tokenService;
        _attemptRepository = attemptRepository;
        _eventPublisher = eventPublisher;
    }
    
    public async Task<QuizResponseDTO> CreateQuizAsync(CreatingQuizRequestDTO request, Guid creatorId)
    {
        var quiz = CreateNewQuiz(request, creatorId);
        await _quizRepository.AddQuizAsync(quiz);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.MapToResponseDTO(quiz);
    }
    
    public async Task<IEnumerable<QuizResponseDTO>> GetAllQuizzesAsync()
    {
        var quizzes = await _quizRepository.GetAllQuizzesAsync();
        return quizzes.Select(q => _mapper.MapToResponseDTO(q));
    }

    public async Task UpdateQuizAsync(Guid quizId, QuizUpdateRequestDTO request)
    {
        var quiz = await _quizRepository.FindByIdAsync(quizId)
                   ?? throw new KeyNotFoundException($"Quiz with ID '{quizId}' not found");

        UpdateQuizProperties(quiz, request);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteQuizAsync(Guid quizId)
    {
        var quiz = await _quizRepository.FindByIdAsync(quizId)
                   ?? throw new KeyNotFoundException($"Quiz with ID '{quizId}' not found");
            
        await _quizRepository.RemoveAsync(quiz);
        await _unitOfWork.SaveChangesAsync();
    }

    //not mean redis just business logic
    public async Task<QuizResponseDTO> PublishQuizAsync(Guid quizId, string token)
    {
        var quiz = await _quizRepository.GetWithQuestionsAndOptionsByIdAsync(quizId)
                   ?? throw new KeyNotFoundException($"Quiz with ID '{quizId}' not found");
        
        quiz.Publish();
        
        //token
        await _quizIntegrationEventService.PublishQuizStartedAsync(quiz, token);
        await _unitOfWork.SaveChangesAsync();
    
        return _mapper.MapToResponseDTO(quiz);
    }
    
    public async Task GetQuizByIdAsync(Guid quizId)
    {
        await _quizRepository.FindByIdAsync(quizId);
    }
    
    //TODO: implement this to controller
    public async Task<FinishQuizResponseDTO> FinishQuizAsync(Guid attemptId, string token)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId)
                      ?? throw new AttemptNotFoundException(attemptId);

        await _quizIntegrationEventService.PublishQuizFinishedAsync(attempt, token);
        
        FinishAttemptAndSaveChanges(attempt);
        
        return _mapper.MapToFinishQuizResponseDTO(attempt);
    }

    private static Quiz CreateNewQuiz(CreatingQuizRequestDTO request, Guid creatorId)
    {
        return new Quiz(creatorId, request.Name, request.Description, request.PassPercentage);
    }

    private static void UpdateQuizProperties(Quiz quiz, QuizUpdateRequestDTO request)
    {
        quiz.UpdateBasicInfo(request.Name, request.Description, request.Category, request.PassPercentage);
    }

    private async Task FinishAttemptAndSaveChanges(QuizAttempt attempt)
    {
        attempt.Finish();
        await _unitOfWork.SaveChangesAsync();
    }
}