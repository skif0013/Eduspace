namespace QuizService.Application.DTOs.QuestionsDTOs;

public class AnswerOptionCreateRequestDTO
{
    public string Text { get; set; }
    
    public bool IsCorrect { get; set; }
    
    public int Order { get; set; }
    
    public double Score { get; set; }
}