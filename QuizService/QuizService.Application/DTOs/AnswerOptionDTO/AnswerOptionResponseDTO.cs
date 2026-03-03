namespace QuizService.Application.DTOs.AnswerOptionDTO;

public class AnswerOptionResponseDTO
{
    public Guid Id { get; set; }
    
    public string Text { get; set; } = null!;
    
    public bool IsCorrectAnswer { get; set; }
    
    public double Score { get; set; }
    
    public int Order { get; set; }
}