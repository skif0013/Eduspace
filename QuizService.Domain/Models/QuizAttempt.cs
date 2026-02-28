using QuizService.Domain.Enum;

namespace QuizService.Domain.Models;

public class QuizAttempt
{
    public Guid Id { get; set; }

    public Guid QuizId { get; set; }
    
    public Quiz Quiz { get; set; }

    public Guid UserId { get; set; }

    public DateTime StartedAt { get; set; }
    
    public DateTime? FinishedAt { get; set; }

    public int Score { get; set; }
    
    public double TotalScore { get; set; }
    public AttemptStatus Status { get; set; }

    public ICollection<UserAnswer> Answers { get; set; } = new List<UserAnswer>();

    public double CalculatePercentage()
    {
        if(Quiz.MaxScore == 0) return 0;
        
        return (TotalScore / Quiz.MaxScore) * 100;
    }
    public bool IsPassed()
    {
        return CalculatePercentage()  >= Quiz.PassPercentage;
    }
}