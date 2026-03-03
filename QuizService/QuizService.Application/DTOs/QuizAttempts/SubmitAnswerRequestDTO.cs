namespace QuizService.Application.DTOs.QuizDTOs;

public class SubmitAnswerRequestDTO
{
    public Guid QuestionId { get; set; }
    
    public List<Guid>? SelectedOptionIds { get; set; }
    
    public string? TextAnswer { get; set; }
}