using QuizService.Application.Contracts;
using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuestionsDTOs.ResponseDTO;
using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Domain.Models;

namespace QuizService.Application.Services;

public class QuizMapper : IQuizMapper
{
    public QuizResponseDTO MapToResponseDTO(Quiz quiz)
    {
        return new QuizResponseDTO
        {
            Id = quiz.Id,
            
            Name = quiz.Name,
            
            Description = quiz.Description,
            
            Category = quiz.Category,
            
            PassPercentage = quiz.PassPercentage,
            
            MaxScore = quiz.MaxScore,
            
            QuestionsCount = quiz.Questions?.Count ?? 0,
        
            IsPublished = quiz.IsPublished,
            
            IsActive = quiz.IsActive,
            
            CreatedOn = quiz.CreatedOn,
            
            ModifiedOn = quiz.ModifiedOn,
            
            Questions = quiz.Questions?
                .Select(q => MapQuestionToResponseDTO(q)) 
                .ToList()
        };
    }

    public QuestionResponseDTO MapQuestionToResponseDTO(Question question)
    {
        return new QuestionResponseDTO
        {
            QuestionId = question.Id,
            Text = question.Text,
            Order = question.Order,
            MaxScore = question.MaxScore,
            QuestionType = question.QuestionType,
            IsActive = question.IsActive
        };
    }

    public QuestionForAttemptDTO MapToQuestionForAttemptDTO(Question question)
    {
        return new QuestionForAttemptDTO()
        {
            QuestionId = question.Id,
            Text = question.Text,
            QuestionType = question.QuestionType,
            Order = question.Order,
            Options = question.AnswerOptions
                .OrderBy(o => o.Order)
                .Select(o => new OptionForAttemptDTO
                {
                    OptionId = o.Id,
                    Text = o.Text,
                    Order = o.Order
                }).ToList()
        };
    }

    public QuizStartResponseDTO ToStartResponseDTO(QuizAttempt attempt, IEnumerable<Question> questions)
    {
        var question = questions.ToList();

        return new QuizStartResponseDTO()
        {
            AttemptId = attempt.Id,
            StartedAt = attempt.StartedAt,
            TotalQuestions = question.Count,
            Questions = question
                .OrderBy(q => q.Order)
                .Select(MapToQuestionForAttemptDTO)
                .ToList()
        };
    }
    
    public FinishQuizResponseDTO MapToFinishQuizResponseDTO(QuizAttempt attempt)
    {
        return new FinishQuizResponseDTO()
        {
            AttemptId = attempt.Id,
            TotalScore = attempt.TotalScore,
            
            TotalQuestions = attempt.Quiz?.Questions.Count ?? 0,
            
            Percentage = Math.Round(attempt.CalculatePercentage(), 2),
            
            IsPassed = attempt.IsPassed(),
        
            FinishedAt = attempt.FinishedAt ?? DateTime.UtcNow
        };
    }
}