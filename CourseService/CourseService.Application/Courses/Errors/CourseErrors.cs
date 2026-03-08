using CourseService.Domain.Abstractions;

namespace CourseService.Application.Courses.Errors;

public static class CourseErrors
{
    public static readonly Error CourseNotFound =
        new("COURSE_NOT_FOUND", "Course was not found");

    public static readonly Error NotCourseAuthor =
        new("NOT_COURSE_AUTHOR", "You are not the author of this course.");
}
