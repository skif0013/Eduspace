using QuizService.Application.DTOs.QuestionsDTOs.ResponseDTO;

namespace QuizService.Application.DTOs.QuizDTOs.ResponeDTO;

public class QuizResponseDTO
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    
    public double PassPercentage { get; set; }
    
    public double MaxScore { get; set; }
    
    public int QuestionsCount { get; set; }
    
    public bool IsPublished { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime ModifiedOn { get; set; }

    
    public List<QuestionResponseDTO>? Questions { get; set; }
}