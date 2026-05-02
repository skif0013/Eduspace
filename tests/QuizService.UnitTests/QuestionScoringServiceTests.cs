using QuizService.Application.Services;
using QuizService.Domain.Enum;

namespace QuizService.UnitTests;

public class QuestionScoringServiceTests
{
    private readonly QuestionScoringService _service = new();

    [Fact]
    public void CalculateScore_SingleChoice_ReturnsMaxScore_ForCorrectSelection()
    {
        var question = TestData.CreateQuestion(
            Guid.NewGuid(),
            maxScore: 10,
            questionType: QuestionType.SingleChoice,
            options: [("Wrong", false, 0, 1), ("Correct", true, 10, 2)]);

        var correctId = question.AnswerOptions.Single(o => o.IsCorrectAnswer).Id;

        var score = _service.CalculateScore(question, [correctId]);

        Assert.Equal(10, score);
    }

    [Fact]
    public void CalculateScore_SingleChoice_ReturnsZero_ForMultipleSelections()
    {
        var question = TestData.CreateQuestion(
            Guid.NewGuid(),
            maxScore: 10,
            questionType: QuestionType.SingleChoice,
            options: [("Wrong", false, 0, 1), ("Correct", true, 10, 2)]);

        var ids = question.AnswerOptions.Select(o => o.Id).ToArray();

        var score = _service.CalculateScore(question, ids);

        Assert.Equal(0, score);
    }

    [Fact]
    public void CalculateScore_MultipleChoice_SumsCorrectSelections_AndCapsAtMaxScore()
    {
        var question = TestData.CreateQuestion(
            Guid.NewGuid(),
            maxScore: 10,
            questionType: QuestionType.MultipleChoice,
            options:
            [
                ("A", true, 4, 1),
                ("B", true, 8, 2),
                ("C", false, 0, 3)
            ]);

        var correctIds = question.AnswerOptions.Where(o => o.IsCorrectAnswer).Select(o => o.Id).ToArray();

        var score = _service.CalculateScore(question, correctIds);

        Assert.Equal(10, score);
    }

    [Fact]
    public void CalculateScore_MultipleChoice_ReturnsZero_WhenWrongOptionSelected()
    {
        var question = TestData.CreateQuestion(
            Guid.NewGuid(),
            maxScore: 10,
            questionType: QuestionType.MultipleChoice,
            options:
            [
                ("A", true, 4, 1),
                ("B", false, 0, 2)
            ]);

        var wrongId = question.AnswerOptions.First(o => !o.IsCorrectAnswer).Id;

        var score = _service.CalculateScore(question, [wrongId]);

        Assert.Equal(0, score);
    }

    [Fact]
    public void CalculateScore_TextAnswer_UsesBooleanFlag()
    {
        var question = TestData.CreateQuestion(
            Guid.NewGuid(),
            maxScore: 7,
            questionType: QuestionType.TextAnswer);

        var correctScore = _service.CalculateScore(question, Array.Empty<Guid>(), true);
        var wrongScore = _service.CalculateScore(question, Array.Empty<Guid>());

        Assert.Equal(7, correctScore);
        Assert.Equal(0, wrongScore);
    }

    [Fact]
    public void CalculateScore_WhenQuestionIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _service.CalculateScore(null!, Array.Empty<Guid>()));
    }

    [Fact]
    public void CalculateScore_WhenSelectedOptionsAreNull_ReturnsZero_ForChoiceQuestions()
    {
        var question = TestData.CreateQuestion(
            Guid.NewGuid(),
            maxScore: 10,
            questionType: QuestionType.TrueFalse,
            options: [("True", true, 10, 1)]);

        var score = _service.CalculateScore(question, null!);

        Assert.Equal(0, score);
    }
}

