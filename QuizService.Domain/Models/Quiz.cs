namespace QuizService.Domain.Models;

public class Quiz
{
    public Guid QuizId {get; set;}
    
    public Guid CreatorId { get; set; }
    
    public string? Category { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public int PassScore { get; set; }
    
    public bool IsActive { get; set; }
    
    public bool IsPublished { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime ModifiedOn { get; set; }
    
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}