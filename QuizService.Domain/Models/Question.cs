using QuizService.Domain.Enum;

namespace QuizService.Domain.Models;

public class Question
{
    
    private Question(){}

    private readonly List<AnswerOption> _answerOptions = new();

    public Question(Guid quizId, string text, int order, int maxScore, QuestionType questionType)
    {
        if (string.IsNullOrWhiteSpace(text)) 
            throw new ArgumentException("Question text cannot be empty.");
            
        if (maxScore < 0)
            throw new ArgumentException("Max score cannot be negative.");

        Id = Guid.NewGuid(); 
        QuizId = quizId;
        Text = text;
        Order = order;
        MaxScore = maxScore;
        QuestionType = questionType;
        CreatedOn = DateTime.UtcNow;
        ModifiedOn = DateTime.UtcNow;
        IsActive = true; 
    }

    public Guid Id { get; set; }

    public Guid QuizId { get; set; }
    public string Text { get; set; } 
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime ModifiedOn { get; set; }
    
    public IReadOnlyCollection<AnswerOption> AnswerOptions => _answerOptions; 

    public void AddAnswerOption(string text, bool isCorrect, double score, int order)
    {
        if(this.QuestionType == QuestionType.SingleChoice && isCorrect && _answerOptions.Any(a => a.IsCorrectAnswer))
        {
            throw new Exception("Single choice question cannot have more than one correct answer.");
        }
        
        if(_answerOptions.Any(o => o.Order == order))
        {
            throw new Exception("An answer option with the same order already exists.");
        }
        
        var option = new AnswerOption(text, isCorrect, score, order);
        
        _answerOptions.Add(option);
        
    }
    
    public void RemoveAnswerOption(AnswerOption option)
    {
        if(option == null) throw new ArgumentNullException(nameof(option));
        _answerOptions.Remove(option);
    }
    
    public int Order { get; set; }
    
    public int MaxScore { get; set; }
    
    public QuestionType QuestionType { get; set; }
    
    public bool IsActive { get; set; }
}