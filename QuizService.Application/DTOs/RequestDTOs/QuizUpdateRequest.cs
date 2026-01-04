namespace QuizService.Application.DTOs;

public class QuizUpdateRequest
{
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public string Category { get; set; }
    
    public int TimeLimit { get; set; }
    
    public int MaxScore { get; set; }
}