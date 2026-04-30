using QuizService.Application.Services;
using QuizService.Domain.Enum;

namespace QuizService.UnitTests;

public class MapperTests
{
    [Fact]
    public void QuizMapper_MapToResponseDTO_IncludesNestedQuestions()
    {
        var mapper = new QuizMapper();
        var quiz = TestData.CreateQuiz(name: "Mapped quiz");
        TestData.AttachQuestions(
            quiz,
            TestData.CreateQuestion(quiz.Id, text: "Q1", order: 2, options: [("A", true, 10, 1)]),
            TestData.CreateQuestion(quiz.Id, text: "Q2", order: 1, options: [("B", true, 10, 1)]));

        var dto = mapper.MapToResponseDTO(quiz);

        Assert.Equal(2, dto.QuestionsCount);
        Assert.Equal("Mapped quiz", dto.Name);
        Assert.Equal(2, dto.Questions!.Count);
        Assert.Contains(dto.Questions, q => q.Text == "Q1");
        Assert.Contains(dto.Questions, q => q.Text == "Q2");
    }

    [Fact]
    public void QuizMapper_ToStartResponseDTO_OrdersQuestions_AndOptions()
    {
        var mapper = new QuizMapper();
        var quiz = TestData.CreateQuiz(name: "Start quiz");
        var question1 = TestData.CreateQuestion(quiz.Id, text: "Second", order: 2, questionType: QuestionType.MultipleChoice,
            options: [("B", true, 4, 2), ("A", true, 3, 1)]);
        var question2 = TestData.CreateQuestion(quiz.Id, text: "First", order: 1, options: [("C", true, 10, 1)]);
        var attempt = TestData.CreateAttempt(quiz.Id);

        var dto = mapper.ToStartResponseDTO(attempt, [question1, question2]);

        Assert.Equal(2, dto.TotalQuestions);
        Assert.Equal("First", dto.Questions[0].Text);
        Assert.Equal("Second", dto.Questions[1].Text);
        Assert.Equal("A", dto.Questions[1].Options![0].Text);
        Assert.Equal("B", dto.Questions[1].Options![1].Text);
    }

    [Fact]
    public void QuizMapper_MapToFinishQuizResponseDTO_UsesAttemptState()
    {
        var mapper = new QuizMapper();
        var quiz = TestData.CreateQuiz(passPercentage: 60);
        TestData.AttachQuestions(quiz, TestData.CreateQuestion(quiz.Id, maxScore: 10));
        var attempt = TestData.CreateAttempt(quiz.Id);
        attempt.Quiz = quiz;
        attempt.TotalScore = 6;
        attempt.Finish();

        var dto = mapper.MapToFinishQuizResponseDTO(attempt);

        Assert.Equal(attempt.Id, dto.AttemptId);
        Assert.Equal(6, dto.TotalScore);
        Assert.Equal(1, dto.TotalQuestions);
        Assert.True(dto.IsPassed);
        Assert.True(dto.Percentage >= 60);
    }

    [Fact]
    public void QuestionMapper_ToResponse_IncludesAnswerOptions()
    {
        var mapper = new QuestionMapper();
        var question = TestData.CreateQuestion(
            Guid.NewGuid(),
            text: "Question",
            options: [("A", true, 10, 2), ("B", false, 0, 1)]);

        var dto = mapper.ToResponse(question);

        Assert.Equal(question.Id, dto.QuestionId);
        Assert.Equal(question.QuizId, dto.QuizId);
        Assert.Equal(2, dto.Options.Count);
        Assert.Equal("A", dto.Options[0].Text);
        Assert.Equal("B", dto.Options[1].Text);
        Assert.Equal(2, dto.Options[0].Order);
        Assert.Equal(1, dto.Options[1].Order);
    }
}

