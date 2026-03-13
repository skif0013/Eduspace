namespace CourseService.Application.Courses.Events;

public record CourseArchivedEvent(Guid CourseId, Guid AuthorId);
