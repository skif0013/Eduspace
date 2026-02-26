using CourseService.Domain.Abstractions;

namespace CourseService.Application.Courses.Errors;

public static class CourseRatingErrors
{
    public static readonly Error RatingAlreadyExists =
        new("RATING_ALREADY_EXISTS", "You have already rated this course.");

    public static readonly Error RatingNotFound =
        new("COURSE_RATING_NOT_FOUND", "Rating was not found.");
}
