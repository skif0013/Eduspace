using QuizService.Domain.Enum;
using QuizService.Domain.Models;

namespace QuizService.UnitTests;

public class DomainModelTests
{
    [Fact]
    public void Quiz_Constructor_ValidatesNameAndPassPercentage()
    {
        Assert.Throws<ArgumentException>(() => new Quiz(Guid.NewGuid(), "", null, 50));
        Assert.Throws<ArgumentException>(() => new Quiz(Guid.NewGuid(), "Quiz", null, -1));
        Assert.Throws<ArgumentException>(() => new Quiz(Guid.NewGuid(), "Quiz", null, 101));
    }

    [Fact]
    public void Quiz_UpdateBasicInfo_ChangesEditableFields()
    {
        var quiz = TestData.CreateQuiz(name: "Old", description: "Old desc", passPercentage: 20);

        quiz.UpdateBasicInfo("New", "New desc", "Category", 90);

        Assert.Equal("New", quiz.Name);
        Assert.Equal("New desc", quiz.Description);
        Assert.Equal(90, quiz.PassPercentage);
    }

    [Fact]
    public void Quiz_Publish_Throws_WithoutQuestions()
    {
        var quiz = TestData.CreateQuiz();

        Assert.Throws<InvalidOperationException>(quiz.Publish);
    }

    [Fact]
    public void Quiz_Publish_SetsFlags_WhenQuestionsExist()
    {
        var quiz = TestData.CreateQuiz();
        TestData.AttachQuestions(quiz, TestData.CreateQuestion(quiz.Id));

        quiz.Publish();

        Assert.True(quiz.IsPublished);
        Assert.True(quiz.IsActive);
    }

    [Fact]
    public void Quiz_MaxScore_SumsQuestionScores()
    {
        var quiz = TestData.CreateQuiz();
        TestData.AttachQuestions(
            quiz,
            TestData.CreateQuestion(quiz.Id, maxScore: 7),
            TestData.CreateQuestion(quiz.Id, maxScore: 5));

        Assert.Equal(12, quiz.MaxScore);
    }

    [Fact]
    public void Question_Constructor_ValidatesTextAndScore()
    {
        Assert.Throws<ArgumentException>(() => new Question(Guid.NewGuid(), "", 1, 1, QuestionType.SingleChoice));
        Assert.Throws<ArgumentException>(() => new Question(Guid.NewGuid(), "Question", 1, -1, QuestionType.SingleChoice));
    }

    [Fact]
    public void Question_AddAnswerOption_RejectsDuplicateOrder()
    {
        var question = TestData.CreateQuestion(Guid.NewGuid(), questionType: QuestionType.MultipleChoice, options: [("A", true, 1, 1)]);

        var ex = Assert.Throws<Exception>(() => question.AddAnswerOption("B", false, 0, 1));

        Assert.Equal("An answer option with the same order already exists.", ex.Message);
    }

    [Fact]
    public void Question_AddAnswerOption_RejectsSecondCorrectOption_ForSingleChoice()
    {
        var question = TestData.CreateQuestion(Guid.NewGuid(), questionType: QuestionType.SingleChoice, options: [("A", true, 1, 1)]);

        var ex = Assert.Throws<Exception>(() => question.AddAnswerOption("B", true, 1, 2));

        Assert.Equal("Single choice question cannot have more than one correct answer.", ex.Message);
    }

    [Fact]
    public void AnswerOption_Constructor_ValidatesTextAndScore()
    {
        Assert.Throws<ArgumentException>(() => new AnswerOption("", true, 1, 1));
        Assert.Throws<ArgumentException>(() => new AnswerOption("A", true, -1, 1));
    }

    [Fact]
    public void AnswerOption_Update_ChangesFields()
    {
        var option = new AnswerOption("A", false, 0, 1);

        option.Update("B", true, 5, 2);

        Assert.Equal("B", option.Text);
        Assert.True(option.IsCorrectAnswer);
        Assert.Equal(5, option.Score);
        Assert.Equal(2, option.Order);
    }

    [Fact]
    public void QuizAttempt_AddAnswer_AccumulatesScore_AndStoresAnswer()
    {
        var attempt = TestData.CreateAttempt(Guid.NewGuid());

        attempt.AddAnswer(Guid.NewGuid(), [Guid.NewGuid()], null!, 8, true);

        Assert.Equal(8, attempt.TotalScore);
        Assert.Single(attempt.Answers);
    }

    [Fact]
    public void QuizAttempt_AddAnswer_WhenNotInProgress_Throws()
    {
        var attempt = TestData.CreateAttempt(Guid.NewGuid());
        attempt.Finish();

        Assert.Throws<InvalidOperationException>(() => attempt.AddAnswer(Guid.NewGuid(), [Guid.NewGuid()], null!, 1, true));
    }

    [Fact]
    public void QuizAttempt_CalculatePercentage_UsesQuizMaxScore()
    {
        var quiz = TestData.CreateQuiz(passPercentage: 60);
        TestData.AttachQuestions(quiz, TestData.CreateQuestion(quiz.Id, maxScore: 10));
        var attempt = TestData.CreateAttempt(quiz.Id);
        attempt.Quiz = quiz;
        attempt.TotalScore = 5;

        Assert.Equal(50, attempt.CalculatePercentage());
        Assert.False(attempt.IsPassed());
    }

    [Fact]
    public void QuizAttempt_Finish_SetsStatusAndTimestamp()
    {
        var attempt = TestData.CreateAttempt(Guid.NewGuid());

        attempt.Finish();

        Assert.Equal(AttemptStatus.Completed, attempt.Status);
        Assert.NotNull(attempt.FinishedAt);
    }
}

