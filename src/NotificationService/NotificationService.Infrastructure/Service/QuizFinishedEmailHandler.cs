using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using NotificationService.Infrastructure.Redis;
using QuizService.Application.DTOs.EventDTOs;

namespace NotificationService.Infrastructure.Services;
public class QuizFinishedEmailHandler : ScopedMessageHandler
{
    public QuizFinishedEmailHandler(IServiceScopeFactory scopeFactory)
        : base(scopeFactory)
    {
    }

    public override string Channel => "quiz:finished:v1";

    protected override async Task HandleScopedAsync(string message, IServiceProvider serviceProvider)
    {
        var emailService = serviceProvider.GetRequiredService<IEmailService>();

        var quizEvent = DeserializeEvent(message);
        if (quizEvent is null)
            return;

        var emailDto = CreateEmailDto(quizEvent);
        await emailService.SendEmailAsync(emailDto);
    }

    private static QuizFinishedEvent? DeserializeEvent(string message)
    {
        return JsonSerializer.Deserialize<QuizFinishedEvent>(message);
    }

    private static EmailSendDTO CreateEmailDto(QuizFinishedEvent quizEvent)
    {
        return new EmailSendDTO
        {
            To = quizEvent.UserEmail,
            Subject = CreateSubject(quizEvent.IsPassed),
            Body = CreateEmailBody(quizEvent)
        };
    }

    private static string CreateSubject(bool isPassed)
    {
        return isPassed ? "Quiz Completed Successfully! ✓" : "Quiz Completed - Review Results ✗";
    }

    private static string CreateEmailBody(QuizFinishedEvent quizEvent)
    {
        var status = quizEvent.IsPassed ? "PASSED ✓" : "FAILED ✗";
        var statusStyle = quizEvent.IsPassed ? "color: green;" : "color: red;";

        return $@"
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Quiz Completion Report</h2>
                    <p><strong style='{statusStyle}'>Status: {status}</strong></p>
                    <p><strong>Your Score:</strong> {quizEvent.TotalScore}</p>
                    <p><strong>Completed At:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss UTC}</p>
                    <hr />
                    <p>Thank you for taking the quiz!</p>
                    <p><em>Eduspace Team</em></p>
                </body>
            </html>";
    }
}
