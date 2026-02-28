using QuizService.Domain.Models;

namespace QuizService.Application.Repositories;

public interface IQuizRepository
{
    Task<IReadOnlyCollection<Quiz>> GetAllQuizzesAsync();
    
    Task AddQuizAsync(Quiz quiz);
    
    Task<Quiz?> FindByIdAsync(Guid quizId);
    Task RemoveAsync(Quiz quiz);
    
    Task<Quiz?> GetWithQuestionsAndOptionsByIdAsync(Guid quizId);
}