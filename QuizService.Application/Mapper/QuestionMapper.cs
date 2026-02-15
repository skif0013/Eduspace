using QuizService.Application.Contracts;
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
            Text = question.Text,
            Order = question.Order,
            MaxScore = question.MaxScore,
            QuestionType = question.QuestionType,
            IsActive = question.IsActive
,            QuizId = question.QuizId
        };
    }
}