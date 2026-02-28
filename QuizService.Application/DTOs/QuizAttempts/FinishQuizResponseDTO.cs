namespace QuizService.Application.DTOs.QuizDTOs;

public class FinishQuizResponseDTO
{
    public Guid AttemptId { get; set; }
    
    public double TotalScore { get; set; }
    
    public int TotalQuestions { get; set; }
    
    public DateTime FinishedAt { get; set; }
    
    public double Percentage { get; set; }
    
    public bool IsPassed { get; set; }
}