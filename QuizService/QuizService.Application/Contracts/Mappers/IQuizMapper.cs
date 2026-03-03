using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuestionsDTOs.ResponseDTO;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Domain.Models;
using QuizService.Application.DTOs.QuizDTOs;

namespace QuizService.Application.Contracts
{
    public interface IQuizMapper
    {
        public QuizResponseDTO MapToResponseDTO(Quiz quiz);
        
        QuizStartResponseDTO ToStartResponseDTO(QuizAttempt attempt, IEnumerable<Question> questions);
        
        QuestionForAttemptDTO MapToQuestionForAttemptDTO(Question question);
        
        public QuestionResponseDTO MapQuestionToResponseDTO(Question question);
        
        FinishQuizResponseDTO MapToFinishQuizResponseDTO(QuizAttempt attempt);
    }
}
