using QuizService.Application.DTOs.QuestionsDTOs.CreateRequestDTOs;
using QuizService.Application.DTOs.QuestionsDTOs.ResponseDTO;

namespace QuizService.Application.Contracts.QuestionsContract;

public interface IQuestionService
{
    Task<QuestionResponseDTO> AddQuestionToQuizAsync(CreateQuestionRequestDTO requestDTO);
    
    Task<QuestionResponseDTO> UpdateQuestionToQuizAsync(CreateQuestionRequestDTO requestDto, Guid questionId);
    
    Task<bool> DeleteQuestionFromQuizAsync(Guid questionId);
    
    Task<QuestionResponseDTO> GetQuestionByIdAsync(Guid questionId);
    
    Task<IReadOnlyCollection<QuestionResponseDTO>> GetAllQuestionsAsync();
}