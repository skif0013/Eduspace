using QuizService.Application.Contracts;
using QuizService.Domain.Enum;
using QuizService.Domain.Models;

namespace QuizService.Application.Services;

public class QuestionScoringService : IQuestionScoringService
{
    public double CalculateScore(
        Question question,
        IReadOnlyCollection<Guid> selectedOptionIds,
        bool isTextAnswerCorrect = false)
    {
        if (question == null)
            throw new ArgumentNullException(nameof(question));

        if (question.QuestionType == QuestionType.TextAnswer)
            return isTextAnswerCorrect ? question.MaxScore : 0;

        var correctOptions = question.AnswerOptions
            .Where(o => o.IsCorrectAnswer)
            .ToList();

        if (question.QuestionType == QuestionType.SingleChoice ||
            question.QuestionType == QuestionType.TrueFalse)
        {
            return correctOptions.Any(o => selectedOptionIds.Contains(o.Id))
                ? question.MaxScore
                : 0;
        }

        if (question.QuestionType == QuestionType.MultipleChoice)
        {
            var score = correctOptions
                .Where(o => selectedOptionIds.Contains(o.Id))
                .Sum(o => o.Score);

            return Math.Min(score, question.MaxScore);
        }

        return 0;
    }
}