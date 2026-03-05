namespace CourseService.Application.Events;

public record CourseCreatedEvent(Guid CourseId, Guid AuthorId);
