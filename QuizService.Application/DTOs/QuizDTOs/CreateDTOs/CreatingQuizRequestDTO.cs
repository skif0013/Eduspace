using QuizService.Domain.Enum;

namespace QuizService.Application.DTOs;

public class CreatingQuizRequestDTO
{
    public string QuizName { get; set; }
    
    public string Description { get; set; }
    
    public string Text { get; set; }
    
    public QuestionType questionType { get; set; }
    
    public string Category { get; set; }
}