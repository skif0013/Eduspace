using QuizService.Domain.Enum;

namespace QuizService.Application.DTOs.QuestionsDTOs.CreateRequestDTOs;

public class CreateQuestionRequestDTO
{
    public string Text { get; set; }
    
    public int Order { get; set; }
    
    public int MaxScore { get; set; }
    
    public QuestionType QuestionType { get; set; }
}