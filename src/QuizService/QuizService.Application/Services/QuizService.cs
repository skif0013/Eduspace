
using QuizService.Application.Contracts;
using BuildingBlock.UserContextMiddleware.Models;
using BuildingBlocks.Redis.Contracts;
using BuildingBlocks.Redis.Events;
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
    private readonly UserContext _userContext;
    private readonly IAttemptRepository _attemptRepository;
    


    public QuizService(IQuizRepository quizRepository, IUnitOfWork unitOfWork, IQuizMapper mapper,
        UserContext userContext, IAttemptRepository attemptRepository, IQuizFinishedEventPublisher eventPublisher)
    {
        _quizRepository = quizRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
        _attemptRepository = attemptRepository;
    }
    
    public async Task<QuizResponseDTO> CreateQuizAsync(CreatingQuizRequestDTO request)

    {
        var userId = _userContext.UserId;
        var quiz = CreateNewQuiz(request, userId);
        await _quizRepository.AddQuizAsync(quiz);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.MapToResponseDTO(quiz);
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
    
    public async Task<FinishQuizResponseDTO> FinishQuizAsync(Guid attemptId)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId)
                      ?? throw new AttemptNotFoundException(attemptId);
        await FinishAttemptAndSaveChanges(attempt);
        
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