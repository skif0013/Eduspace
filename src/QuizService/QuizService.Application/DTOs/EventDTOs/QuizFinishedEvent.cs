namespace QuizService.Application.DTOs.EventDTOs;

public record QuizFinishedEvent(
        Guid AttemptId,
        string UserEmail,
        string TotalScore,
        bool IsPassed
    );

