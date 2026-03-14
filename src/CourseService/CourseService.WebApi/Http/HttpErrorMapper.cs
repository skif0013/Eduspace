using CourseService.Application.Courses.Errors;
using CourseService.Domain.Abstractions;

namespace CourseService.WebApi.Http;

public static class HttpErrorMapper
{
    private static readonly Dictionary<Error, int> _map = new()
    {
        { CourseErrors.CourseNotFound, StatusCodes.Status404NotFound },
        { CourseErrors.CourseArchived, StatusCodes.Status409Conflict },
        { CourseErrors.NotCourseAuthor, StatusCodes.Status403Forbidden },
        { CourseErrors.CourseRequiresPayment, StatusCodes.Status403Forbidden },

        { CourseRatingErrors.RatingAlreadyExists, StatusCodes.Status409Conflict },
        { CourseRatingErrors.RatingNotFound, StatusCodes.Status404NotFound },

        { LessonErrors.LessonNotFound, StatusCodes.Status404NotFound },
        { LessonErrors.NotLessonAuthor, StatusCodes.Status403Forbidden }
    };

    public static int Map(Error error)
    {
        if (_map.TryGetValue(error, out var statusCode))
        {
            return statusCode;
        }

        return StatusCodes.Status400BadRequest;
    }
}
