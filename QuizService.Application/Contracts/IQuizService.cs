using QuizService.Domain.Models;
using QuizService.Application.DTOs;
namespace QuizService.Application.Contracts;

public interface IQuizService
{
    Task<Quiz> GetQuiz(Guid QuizId);
    
    Task<List<Quiz>> GetQuizzes();
    
    Task<Quiz> AddQuiz(CretingQuizDTO response, Guid userId);
    
    Task<Quiz> UpdateQuiz(Guid quizId, CretingQuizDTO response);
    
    Task<Quiz> DeleteQuiz(Guid QuizId);
    
    Task<Quiz> FindByIdAsync(Guid QuizId, Guid userId);
}