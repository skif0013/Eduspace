using QuizService.Application.DTOs.QuizDTOs;

namespace QuizService.Application.Contracts.IQuizAttempt;

public interface IAttemptService
{
    Task<QuizStartResponseDTO> StartQuizAsync(Guid quizId, Guid userId);

    Task<SubmitAnswerResponseDTO> SubmitAnswerAsync(Guid attemptId, SubmitAnswerRequestDTO request);
}