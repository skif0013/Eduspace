using QuizService.Domain.Enum;

namespace QuizService.Application.DTOs.QuizDTOs;

public class QuestionForAttemptDTO
{
    public Guid QuestionId { get; set; }
    
    public string Text { get; set; } = null!;
    
    public QuestionType QuestionType { get; set; }
    
    public int Order { get; set; }

    public List<OptionForAttemptDTO>? Options { get; set; }
}