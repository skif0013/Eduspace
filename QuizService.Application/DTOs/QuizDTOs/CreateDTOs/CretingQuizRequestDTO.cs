using QuizService.Domain.Enum;

namespace QuizService.Application.DTOs;

public class CretingQuizRequestDTO
{
    public string QuizName { get; set; }
    
    public string Text { get; set; }
    
    public QuestionType questionType { get; set; }
    
    public string Category { get; set; }
}