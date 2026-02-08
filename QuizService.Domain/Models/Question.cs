using QuizService.Domain.Enum;

namespace QuizService.Domain.Models;

public class Question
{

    public Guid Id { get; set; }
    
    public Guid QuizId { get; set; }
    public string Text { get; set; } 
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime ModifiedOn { get; set; }
    
    public int Order { get; set; }
    
    public int MaxScore { get; set; }
    
    public QuestionType QuestionType { get; set; }
    
    public bool IsActive { get; set; }
}