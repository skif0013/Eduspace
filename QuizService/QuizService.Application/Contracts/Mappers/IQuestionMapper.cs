using QuizService.Application.DTOs.QuestionsDTOs.ResponseDTO;
using QuizService.Domain.Models;

namespace QuizService.Application.Contracts;

public interface IQuestionMapper
{
    QuestionResponseDTO ToResponse(Question question);
}