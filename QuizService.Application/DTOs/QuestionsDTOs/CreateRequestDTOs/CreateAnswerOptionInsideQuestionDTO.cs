namespace QuizService.Application.DTOs.QuestionsDTOs.CreateRequestDTOs;

public class CreateAnswerOptionInsideQuestionDTO
{
    public string Text { get; set; }
    
    public bool IsCorrect { get; set; }
    
    public double Score { get; set; }
    
    public int Order { get; set; }
}