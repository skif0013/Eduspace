using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;


namespace QuizService.Application.Contracts;

public interface IQuizService
{
    Task<QuizResponseDTO> CreateQuizAsync(CreatingQuizRequestDTO request, Guid creatorId);
    
    Task UpdateQuizAsync(Guid quizId, QuizUpdateRequestDTO request);
    
    Task<FinishQuizResponseDTO> FinishQuizAsync(Guid quizId, string token);
    
    Task DeleteQuizAsync(Guid quizId);
}