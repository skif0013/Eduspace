using CourseService.Domain.Abstractions;

namespace CourseService.Application.Courses.Errors;

public static class LessonErrors
{
    public static readonly Error LessonNotFound =
        new("LESSON_NOT_FOUND", "Lesson was not found");

    public static readonly Error NotLessonAuthor =
        new("NOT_LESSON_AUTHOR", "You are not the author of this lesson.");
}
