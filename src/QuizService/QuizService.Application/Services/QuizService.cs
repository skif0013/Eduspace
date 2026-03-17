using QuizService.Application.Contracts;
using QuizService.Application.Contracts.IQuizAttempt;
using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
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

    public QuizService(IQuizRepository quizRepository, IUnitOfWork unitOfWork, IQuizMapper mapper,
        ITokenService tokenService,IAttemptRepository attemptRepository)
    {
        _attemptRepository = attemptRepository;
        _tokenService = tokenService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _quizRepository = quizRepository;
    }
    
    public async Task<QuizResponseDTO> CreateQuizAsync(CreatingQuizRequestDTO request, Guid creatorId)
    {
        var quiz = new Quiz(
            creatorId,
            request.Name, 
            request.Description, 
            request.PassPercentage
        );

       
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
                   ?? throw new KeyNotFoundException("quiz not found");

        
        quiz.UpdateBasicInfo(
            request.Name, 
            request.Description, 
            request.Category, 
            request.PassPercentage
        );

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteQuizAsync(Guid quizId)
    {
        var quiz = await _quizRepository.FindByIdAsync(quizId)
                   ?? throw new KeyNotFoundException("quiz not found");
            
        _quizRepository.RemoveAsync(quiz); 
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<QuizResponseDTO> PublishQuizAsync(Guid quizId)
    {
        var quiz = await _quizRepository.GetWithQuestionsAndOptionsByIdAsync(quizId)
                   ?? throw new KeyNotFoundException("quiz not found");
        
        quiz.Publish();

        await _unitOfWork.SaveChangesAsync();

        return _mapper.MapToResponseDTO(quiz);
    }
    
    public async Task GetQuizByIdAsync(Guid quizId)
    {
        await _quizRepository.FindByIdAsync(quizId);
    }
    
    
    public async Task<FinishQuizResponseDTO> FinishQuizAsync(Guid attemptId)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId)
                      ?? throw new Exception("Attempt not found");

        attempt.Finish();
        
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.MapToFinishQuizResponseDTO(attempt);
    }
}