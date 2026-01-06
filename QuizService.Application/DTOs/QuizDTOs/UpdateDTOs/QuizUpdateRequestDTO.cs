namespace QuizService.Application.DTOs;

public class QuizUpdateRequestDTO
{
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string Category { get; set; }
    
    public bool IsActive { get; set; }
    
    public bool IsPublished { get; set; }
    
    public int TimeLimit { get; set; }
    
    public int MaxScore { get; set; }
}