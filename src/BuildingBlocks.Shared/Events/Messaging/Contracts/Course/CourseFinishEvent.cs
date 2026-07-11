using BuildingBlocks.Redis.Events;

namespace BuildingBlocks.Shared.Events.Messaging.Contracts.Course;

public record CourseFinishEvent(
    string UserEmail,
    string UserName,
    Guid CourseId,
    string CourseTitle,
    DateTime FinishDate,
    decimal FinishScore
    ) : IntegrationEvent(new Guid(), DateTime.UtcNow);