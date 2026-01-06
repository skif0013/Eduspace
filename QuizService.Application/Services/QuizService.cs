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

    public QuizService(IQuizRepository quizRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _quizRepository = quizRepository;
    }


    public async Task<QuizResponseDTO> CreateQuizAsync(CreatingQuizRequestDTO request, Guid userId)
    {
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
        
    }

    public async Task<Quiz> FindByIdAsync(Guid userId, Guid quizId)
    {
        
    }
  
    
    //for developming
    public async Task<List<Quiz>> GetQuizzes()
    {
        
    }
    
    public async Task<Quiz?> UpdateQuizAsync(Guid quizId, QuizUpdateRequestDTO request, Guid userId)
    {
        
    }
    
    
    public async Task<Quiz> DeleteQuizAsync(Guid quizId, Guid userId)
    {
        
    }
    
}