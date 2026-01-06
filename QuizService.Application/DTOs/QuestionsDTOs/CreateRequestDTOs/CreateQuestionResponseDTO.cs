using QuizService.Domain.Enum;

namespace QuizService.Application.DTOs.QuestionsDTOs.CreateRequestDTOs;

public class CreateQuestionResponseDTO
{
    public Guid QuestionId { get; set; }
    
    public string Text { get; set; }
    
    public int Order { get; set; }
    
    public int MaxScore { get; set; }
    
    public QuestionType QuestionType { get; set; }
    
    public bool IsActive { get; set; }
    
    
    public IReadOnlyCollection<CreateAnswerOptionResponse> AnswerOption { get; set; }
}