

namespace QuizService.Application.DTOs;

public class CreatingQuizRequestDTO
{
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public string? Category { get; set; }
    
    public double PassPercentage { get; set; }
}