using CourseService.Domain.Abstractions;

namespace CourseService.Application.Courses.Errors;

public static class CourseErrors
{
    public static readonly Error CourseNotFound =
        new("Course.CourseNotFound", "Course was not found");

    public static readonly Error NotCourseAuthor =
        new("Course.NotCourseAuthor", "You are not the author of this course.");
}
