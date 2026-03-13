namespace CourseService.Application.Courses.Events;

public record LessonCreatedEvent(Guid lessonId, Guid courseId, Guid authorId);
