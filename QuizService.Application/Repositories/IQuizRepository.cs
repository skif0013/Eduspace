using QuizService.Domain.Models;

namespace QuizService.Application.Repositories;

public interface IQuizRepository
{
    Task<IEnumerable<Quiz>> GetAllQuizzesAsync();
    
    Task<Quiz> FindByIdAsync(Guid QuizId);
    
    Task<Quiz> AddQuizAsync(Quiz quiz);
    
    Task<Quiz> UpdateQuizAsync(Guid QuizId);
    
    Task<Quiz> DeleteQuizAsync(Guid id);
}