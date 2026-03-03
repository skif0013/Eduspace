using QuizService.Application.Contracts;
using QuizService.Application.DTOs.AnswerOptionDTO;
using QuizService.Application.DTOs.QuestionsDTOs.ResponseDTO;
using QuizService.Domain.Models;

namespace QuizService.Application.Services;

public class QuestionMapper : IQuestionMapper
{
    public QuestionResponseDTO ToResponse(Question question)
    {
        return new QuestionResponseDTO
        {
            QuestionId = question.Id,
            QuizId = question.QuizId,
            Text = question.Text,
            Order = question.Order,
            MaxScore = question.MaxScore,
            QuestionType = question.QuestionType,
            IsActive = question.IsActive,
        
            // Мапим список опций
            Options = question.AnswerOptions.Select(opt => new AnswerOptionResponseDTO
            {
                Id = opt.Id,
                Text = opt.Text,
                IsCorrectAnswer = opt.IsCorrectAnswer, 
                Score = opt.Score,
                Order = opt.Order
            }).ToList()
        };
    }
}