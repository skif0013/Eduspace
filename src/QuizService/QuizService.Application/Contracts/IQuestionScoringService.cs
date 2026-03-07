using QuizService.Domain.Models;

namespace QuizService.Application.Contracts;

public interface IQuestionScoringService
{
    double CalculateScore(Question question, IReadOnlyCollection<Guid> selectedOptionIds, bool isCorrect = false);
}