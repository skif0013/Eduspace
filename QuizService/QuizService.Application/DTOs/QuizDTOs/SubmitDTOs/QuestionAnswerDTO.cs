namespace QuizService.Application.DTOs.QuizDTOs.SubmitDTOs;

public class QuestionAnswerDTO
{
    public Guid QuestionId { get; set; }
    
    public List<Guid> SelectedOptionIds { get; set; }
}