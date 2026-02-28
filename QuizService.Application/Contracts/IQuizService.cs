using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Application.DTOs.QuizDTOs.SubmitDTOs;
using QuizService.Application.DTOs.ResponseDTOs;

namespace QuizService.Application.Contracts;

public interface IQuizService
{
    Task<IReadOnlyCollection<QuizResponseDTO>> GetAllQuizzesAsync();
    
    Task<QuizResponseDTO> GetQuizByIdAsync(Guid userId, Guid quizId);
    
    Task<QuizResponseDTO> CreateQuizAsync(CreatingQuizRequestDTO request);
    
    Task UpdateQuizAsync(Guid quizId, QuizUpdateRequestDTO request);
    
    Task DeleteQuizAsync(Guid quizId);
}