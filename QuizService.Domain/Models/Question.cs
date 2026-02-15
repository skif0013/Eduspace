using QuizService.Domain.Enum;

namespace QuizService.Domain.Models;

public class Question
{

    private readonly List<AnswerOption> _answerOptions = new();
    
    public Guid Id { get; set; }
    
    public Guid QuizId { get; set; }
    public string Text { get; set; } 
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime ModifiedOn { get; set; }
    
    public IReadOnlyCollection<AnswerOption> AnswerOptions => _answerOptions; 

    public void AddAnswerOption(AnswerOption option)
    {
        if(option == null) throw new ArgumentNullException(nameof(option));
        option.QuestionId = this.Id;
        option.Question = this;
        _answerOptions.Add(option);
    }

    public void RemoveAnswerOption(AnswerOption option)
    {
        if(option == null) throw new ArgumentNullException(nameof(option));
        _answerOptions.Remove(option);
    }
    
    public void UpdateAnswerOption(Guid Id, string Text, bool IsCorrect, int Order, double Score)
    {
        
        var existingOption = _answerOptions.FirstOrDefault(o => o.Id == Id);
        
        if (existingOption == null)
        {
            throw new ArgumentException($"Answer option with Id {Id} not found.");
        }
        
        existingOption.Text = Text;
        existingOption.IsCorrectAnswer = IsCorrect;
        existingOption.Order = Order;
        existingOption.Score = Score;
    }
    
    public int Order { get; set; }
    
    public int MaxScore { get; set; }
    
    public QuestionType QuestionType { get; set; }
    
    public bool IsActive { get; set; }
}