namespace QuizService.Application.DTOs.QuizDTOs;

public class OptionForAttemptDTO
{
    public Guid OptionId { get; set; }
    
    public string Text { get; set; } = null!;
    
    public int Order { get; set; }
}