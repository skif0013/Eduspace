using QuizService.Domain.Models;

namespace QuizService.Application.Contracts.QuestionsContract;

public interface IQuestionRepository
{
    Task<IReadOnlyCollection<Question>> GetAllQuestionsAsync();
    
    Task<Question?> FindByIdAsync(Guid questionId);
    
    Task AddQuestion(Question question);
    
    Task RemoveAsync(Question question);
    
    Task<List<Question>> GetActiveByQuizIdAsync(Guid quizId);
    
    Task<Question> GetWithOptionsByIdAsync(Guid questionId);
}