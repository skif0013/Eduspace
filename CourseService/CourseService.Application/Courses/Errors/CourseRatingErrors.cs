using CourseService.Domain.Abstractions;

namespace CourseService.Application.Courses.Errors;

public static class CourseRatingErrors
{
    public static readonly Error RatingAlreadyExists =
        new("CourseRation.RatingAlreadyExists", "You have already rated this course.");

    public static readonly Error RatingNotFound =
        new("CourseRating.RatingNotFound", "Rating was not found.");
}
