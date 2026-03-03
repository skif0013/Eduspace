using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;


namespace QuizService.Application.Contracts;

public interface IQuizService
{
    Task<IEnumerable<QuizResponseDTO>> GetAllQuizzesAsync();
    
    Task<QuizResponseDTO> CreateQuizAsync(CreatingQuizRequestDTO request, Guid creatorId);
    
    Task UpdateQuizAsync(Guid quizId, QuizUpdateRequestDTO request);
    
    Task DeleteQuizAsync(Guid quizId);
    
    Task GetQuizByIdAsync(Guid quizId);
    
    Task<QuizResponseDTO> PublishQuizAsync(Guid quizId);
    
    Task<FinishQuizResponseDTO> FinishQuizAsync(Guid attemptId);
}