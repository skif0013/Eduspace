namespace QuizService.Application.DTOs.QuestionOptionDTO.ResponseDTO;

public class AnswerOptionResponseDTO
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    
    public bool IsCorrect { get; set; }
    
    public int Order { get; set; }
    
    public double Score { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime ModifiedOn { get; set; }
}