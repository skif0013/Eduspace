namespace QuizService.Application.DTOs.QuizDTOs;

public class SubmitAnswerResponseDTO
{
    public bool IsCorrect { get; set; }
    
    public double EarnedScore { get; set; }
}