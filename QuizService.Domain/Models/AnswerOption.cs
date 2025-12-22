namespace QuizService.Domain.Models;

public class AnswerOption
{
    public Guid Id { get; set; }
    
    public Guid QuestionId { get; set; }
    
    public string Text { get; set; }
    
    public bool IsCorrectAnswer { get; set; }
    
    public int Order { get; set; }
    
    
    public decimal? CorrectnessPercentage { get; set; }
}