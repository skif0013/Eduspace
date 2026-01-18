using QuizService.Application.Contracts;
using QuizService.Application.DTOs;
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

    public QuizService(IQuizRepository quizRepository, IUnitOfWork unitOfWork, IQuizMapper mapper, ITokenService tokenService)
    {
        _tokenService = tokenService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _quizRepository = quizRepository;
    }


    public async Task<QuizResponseDTO> CreateQuizAsync(CreatingQuizRequestDTO request)
    {
        
        //temporarily
        Guid userId = Guid.NewGuid();
        
        var quiz = new Quiz()
        {
            QuizId = Guid.NewGuid(),
            CreatorId = userId,
            Description = request.Text,
            IsPublished = true,
            IsActive = true,
            ModifiedOn = DateTime.Now,
            Name = request.QuizName,
            CreatedOn = DateTime.Now,
        };
        
        await _quizRepository.AddQuizAsync(quiz);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.MapToResponseDTO(quiz);
    }

    public async Task<QuizResponseDTO> GetQuizByIdAsync(Guid userId, Guid quizId)
    {
        var find = await _quizRepository.FindByIdAsync(quizId);
        
        if (find == null)
        {
            throw new Exception("Quiz not found");
        }
        
        return _mapper.MapToResponseDTO(find);
    }
  
    
    //for develop 
    public async Task<IReadOnlyCollection<QuizResponseDTO>> GetAllQuizzesAsync()
    {
        var getAll = await _quizRepository.GetAllQuizzesAsync();
        
        return getAll.Select(q => _mapper.MapToResponseDTO(q)).ToList();
    }
    
    public async Task UpdateQuizAsync(Guid quizId, QuizUpdateRequestDTO request)
    {
        var update = await _quizRepository.FindByIdAsync(quizId);
        
        
        if (update == null)
        {
            throw new Exception("Quiz not found");
        }
        
        update.Name = request.Title;
        update.Description = request.Description;
        update.IsActive = request.IsActive;
        update.IsPublished = request.IsPublished;
        update.ModifiedOn = DateTime.Now;
        await _unitOfWork.SaveChangesAsync();
    }
    
    
    public async Task DeleteQuizAsync(Guid quizId)
    {
        var delete = await _quizRepository.FindByIdAsync(quizId);
        
        if (delete == null)
        {
            throw new Exception("Quiz not found");
        }
        
        await _quizRepository.RemoveAsync(delete);
        await _unitOfWork.SaveChangesAsync();
    }
}