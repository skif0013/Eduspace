using QuizService.Domain.Models;
using QuizService.Application.DTOs;
namespace QuizService.Application.Contracts;

public interface IQuizService
{
    
    Task<List<Quiz>> GetQuizzes();
    
    Task<Quiz> AddQuizAsync(CretingQuizRequestDTO response, Guid userId);
    
    Task<Quiz> UpdateQuizAsync(Guid quizId, QuizUpdateRequestDTO request, Guid userId);
    
    Task<Quiz> DeleteQuizAsync(Guid QuizId, Guid userId);
    
    Task<Quiz> FindByIdAsync(Guid QuizId, Guid userId);
}