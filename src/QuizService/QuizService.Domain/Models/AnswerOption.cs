namespace QuizService.Domain.Models;

public class AnswerOption
{
    public Guid Id { get; private set; }
    public Guid QuestionId { get; private set; }
    public string Text { get; private set; }
    public bool IsCorrectAnswer { get; private set; }
    public int Order { get; private set; }
    public double Score { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }

    
    public AnswerOption() {}
    
    
    public AnswerOption(string text, bool isCorrectAnswer, double score, int order)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty", nameof(text));

        if (score < 0)
            throw new ArgumentException("Score cannot be negative", nameof(score));

        Id = Guid.NewGuid();
        Text = text;
        IsCorrectAnswer = isCorrectAnswer;
        Score = score;
        Order = order;
        CreatedOn = DateTime.UtcNow;
        ModifiedOn = DateTime.UtcNow;
    }
}