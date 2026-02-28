using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Domain.Models;
using QuizService.Application.DTOs.QuizDTOs;

namespace QuizService.Application.Contracts
{
    public interface IQuizMapper
    {
        Quiz MapToDomain(CreatingQuizRequestDTO request, Guid userId);

        void MapToDomain(QuizUpdateRequestDTO request, Quiz quiz);

        public QuizResponseDTO MapToResponseDTO(Quiz quiz);
        
        QuizStartResponseDTO ToStartResponseDTO(QuizAttempt attempt, IEnumerable<Question> questions);
        
        QuestionForAttemptDTO MapToQuestionForAttemptDTO(Question question);
        
        FinishQuizResponseDTO MapToFinishQuizResponseDTO(QuizAttempt attempt);
    }
}
