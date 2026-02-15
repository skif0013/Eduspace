namespace QuizService.Application.DTOs.AnswerOptionDTO.UpdateAnswerOption;

public class UpdateAnswerOptionDTO
{
    public Guid Id { get; set; }
    
    public string Text { get; set; }
    
    public bool IsCorrect { get; set; }
    
    public int Order { get; set; }
    
    public double Score { get; set; }

}