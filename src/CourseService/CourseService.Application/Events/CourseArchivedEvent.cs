namespace CourseService.Application.Events;

public record CourseArchivedEvent(Guid CourseId, Guid AuthorId);
