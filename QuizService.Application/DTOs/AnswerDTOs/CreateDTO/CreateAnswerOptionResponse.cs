namespace QuizService.Application.DTOs.QuestionsDTOs;

public class CreateAnswerOptionResponse
{
    public Guid AnswerOptionId { get; set; }
    
    public string Text { get; set; }
    
    public bool IsCorrectAnswer { get; set; }
    
    public int Order { get; set; }
    
    public double Score { get; set; }
}