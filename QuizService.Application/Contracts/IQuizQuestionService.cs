using QuizService.Application.DTOs.QuestionsDTOs.CreateRequestDTOs;
using QuizService.Application.DTOs.QuestionsDTOs.ResponseDTO;
using QuizService.Application.DTOs.QuestionsDTOs.UpdateDTOs;


namespace QuizService.Application.Contracts;

public interface IQuizQuestionService
{
    Task<QuestionResponseDTO> AddQuestionAsync(
        Guid quizId,
        CreateQuestionRequestDTO request);

    Task UpdateQuestionAsync(
        Guid quizId,
        Guid questionId,
        UpdateQuestionRequestDTO request);

    Task RemoveQuestionAsync(
        Guid quizId,
        Guid questionId);

    Task<IReadOnlyCollection<QuestionResponseDTO>> GetQuestionsAsync(
        Guid quizId);
}