namespace CourseService.Application.Courses.Events;

public record CoursePublishedEvent(Guid CourseId, Guid AuthorId);
