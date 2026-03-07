using QuizService.Application.DTOs.AnswerOptionDTO;
using QuizService.Domain.Enum;

namespace QuizService.Application.DTOs.QuestionsDTOs.ResponseDTO;

public class QuestionResponseDTO
{
    public Guid QuestionId { get; set; }
    public Guid QuizId { get; set; }
    public string Text { get; set; } = null!;
    public int Order { get; set; }
    public int MaxScore { get; set; }
    public QuestionType QuestionType { get; set; }
    public bool IsActive { get; set; }
    
    public List<AnswerOptionResponseDTO> Options { get; set; } = new();
}