namespace CourseService.Application.Courses.Events;

public record CourseCreatedEvent(Guid CourseId, Guid AuthorId);
