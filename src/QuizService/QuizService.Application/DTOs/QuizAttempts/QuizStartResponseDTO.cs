using QuizService.Application.DTOs.QuestionsDTOs.ResponseDTO;

namespace QuizService.Application.DTOs.QuizDTOs;

public class QuizStartResponseDTO
{
    public Guid AttemptId { get; set; }
    
    public DateTime StartedAt { get; set; }
    
    public int TotalQuestions { get; set; }
    
    public List<QuestionForAttemptDTO> Questions { get; set; } = new();
}