namespace QuizService.Application.DTOs.QuestionOptionDTO;

public class AnswerOptionDTO
{
    public string Text { get; set; }
    
    public bool IsCorrect { get; set; }
    
    public int Order { get; set; }
    
    public double Score { get; set; }
    
}