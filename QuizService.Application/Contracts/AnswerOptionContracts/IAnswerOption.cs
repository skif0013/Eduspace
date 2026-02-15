using QuizService.Application.DTOs.AnswerOptionDTO.UpdateAnswerOption;
using QuizService.Application.DTOs.QuestionOptionDTO;
using QuizService.Application.DTOs.QuestionOptionDTO.ResponseDTO;

namespace QuizService.Application.Contracts.AnswerOptionContracts;

public interface IAnswerOption
{
    Task<AnswerOptionResponseDTO> AddAnswerOptionAsync(AnswerOptionDTO requestDTO, Guid questionId);
    
    Task UpdateAnswerOptionAsync(UpdateAnswerOptionDTO requestDTO, Guid questionId);
    
    Task<bool> DeleteAnswerOptionAsync(Guid answerOptionId, Guid questionId);
    
    Task<IReadOnlyCollection<AnswerOptionResponseDTO>>GetAnswerOptionsByQuestionIdAsync(Guid questionId);
}