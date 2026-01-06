namespace QuizService.Application.DTOs.QuizDTOs.ResponeDTO;

public class QuizResponseDTO
{
    public Guid QuizId { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Category { get; set; }
    
    public bool IsPublished { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime ModifiedOn { get; set; }
}