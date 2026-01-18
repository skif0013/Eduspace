namespace QuizService.Domain.Models;

public class AnswerOption
{
    public Guid Id { get; set; }
    
    public Question question { get; set; }
    
    public string Text { get; set; }
    
    public bool IsCorrectAnswer { get; set; }
    
    public int Order { get; set; }
    
    public double Score { get; set; }
    
}