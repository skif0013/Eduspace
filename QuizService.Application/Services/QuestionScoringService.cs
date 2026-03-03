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
        if (question == null) throw new ArgumentNullException(nameof(question));
        if (selectedOptionIds == null || !selectedOptionIds.Any() && question.QuestionType != QuestionType.TextAnswer) 
            return 0;

        
        if (question.QuestionType == QuestionType.TextAnswer)
            return isTextAnswerCorrect ? question.MaxScore : 0;

        
        if (question.QuestionType == QuestionType.SingleChoice || question.QuestionType == QuestionType.TrueFalse)
        {
            
            if (selectedOptionIds.Count != 1) return 0;

            var correctOption = question.AnswerOptions.FirstOrDefault(o => o.IsCorrectAnswer);
            return correctOption != null && selectedOptionIds.Contains(correctOption.Id) 
                ? question.MaxScore 
                : 0;
        }

        
        if (question.QuestionType == QuestionType.MultipleChoice)
        {
            var allOptions = question.AnswerOptions;
            var correctOptionIds = allOptions.Where(o => o.IsCorrectAnswer).Select(o => o.Id).ToList();
            
            
            bool hasWrongSelection = selectedOptionIds.Any(id => 
                allOptions.Any(o => o.Id == id && !o.IsCorrectAnswer));
            
            if (hasWrongSelection) return 0; 
            
            var score = allOptions
                .Where(o => o.IsCorrectAnswer && selectedOptionIds.Contains(o.Id))
                .Sum(o => o.Score);

            return Math.Min(score, question.MaxScore);
        }

        return 0;
    }
}