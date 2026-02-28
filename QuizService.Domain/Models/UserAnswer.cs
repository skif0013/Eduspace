namespace QuizService.Domain.Models;

public class UserAnswer
{
    public Guid Id { get; set; }

    public Guid AttemptId { get; set; }
    
    public QuizAttempt Attempt { get; set; }

    public Guid QuestionId { get; set; }

    public string? TextAnswer { get; set; }

    public List<Guid>? SelectedOptionId { get; set; }

    public bool IsCorrect { get; set; }
}