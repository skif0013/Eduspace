using QuizService.Application.Contracts;
using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Domain.Models;

namespace QuizService.Application.Services;

public class QuizMapper : IQuizMapper
{
    public Quiz MapToDomain(CreatingQuizRequestDTO dto, Guid userId)
    {
        return new Quiz
        {
            Id = Guid.NewGuid(),
            CreatorId = userId,
            Name = dto.QuizName,
            Description = dto.Description,
            Category = dto.Category,
            IsActive = true,
            IsPublished = false,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow
        };
    }

    public void MapToDomain(QuizUpdateRequestDTO request, Quiz quiz)
    {
        quiz.Name = request.Title;
        quiz.Description = request.Description;
        quiz.Category = request.Category;
        quiz.IsActive = request.IsActive;
        quiz.IsPublished = request.IsPublished;
        quiz.ModifiedOn = DateTime.UtcNow;
    }
    
    public QuizResponseDTO MapToResponseDTO(Quiz quiz)
    {
        return new QuizResponseDTO
        {
            QuizId = quiz.Id,
            Name = quiz.Name,
            Description = quiz.Description,
            Category = quiz.Category,
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