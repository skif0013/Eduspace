namespace CourseService.Application.Events;

public record CoursePublishedEvent(Guid CourseId, Guid AuthorId);
