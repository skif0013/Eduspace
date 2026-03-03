namespace QuizService.Domain.Models;

public class UserAnswer
{

    public UserAnswer() {}
    
    public UserAnswer(Guid questionId, List<Guid> selectedOptionIds, string? textAnswer, bool selectedOptionId)
    {
        QuestionId = questionId;
        SelectedOptionId = selectedOptionIds;
        TextAnswer = textAnswer;
        IsCorrect = selectedOptionId;
    }

    public Guid Id { get; set; }

    public Guid AttemptId { get; set; }
    
    public QuizAttempt Attempt { get; set; }

    public Guid QuestionId { get; set; }

    public string? TextAnswer { get; set; }

    public List<Guid>? SelectedOptionId { get; set; }

    public bool IsCorrect { get; set; }
}