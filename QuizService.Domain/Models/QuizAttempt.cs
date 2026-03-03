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

    private readonly List<UserAnswer> _answers = new();
    public IReadOnlyCollection<UserAnswer> Answers => _answers.AsReadOnly();
    
    public QuizAttempt(){}
    
    
    public QuizAttempt(Guid quizId, Guid userId)
    {
        Id = Guid.NewGuid();
        QuizId = quizId;
        UserId = userId;
        StartedAt = DateTime.UtcNow;
        Status = AttemptStatus.InProgress;
    }

    public void AddScore(double score)
    {
        if (Status != AttemptStatus.InProgress)
        {
            throw new InvalidOperationException("Cannot add score to a finished or cancelled attempt");
        }
        
        TotalScore += score;
    }
    
    
    public void AddAnswer(Guid questionId, List<Guid> selectedOptionIds, string textAnswer, double score, bool isCorrect)
    {
        if (Status != AttemptStatus.InProgress)
        {
            throw new InvalidOperationException("Cannot add answer to a finished or cancelled attempt");
        }
        
        var answer = new UserAnswer(questionId, selectedOptionIds, textAnswer, isCorrect);
        
        _answers.Add(answer);
        
        this.TotalScore += score;
    }

    public void Finish()
    {
        if (Status != AttemptStatus.Completed)
        {
            return;
        }
        
        FinishedAt = DateTime.UtcNow;
        Status = AttemptStatus.Completed;
    }
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