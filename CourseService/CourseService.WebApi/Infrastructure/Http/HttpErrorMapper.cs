using CourseService.Application.Courses.Errors;
using CourseService.Domain.Abstractions;

namespace CourseService.WebApi.Infrastructure.Http;

public static class HttpErrorMapper
{
    private static readonly Dictionary<string, int> _map = new()
    {
        { CourseErrors.CourseNotFound.Code, StatusCodes.Status404NotFound },
        { CourseErrors.NotCourseAuthor.Code, StatusCodes.Status403Forbidden },
        { CourseRatingErrors.RatingAlreadyExists.Code, StatusCodes.Status409Conflict },
        { CourseRatingErrors.RatingNotFound.Code, StatusCodes.Status404NotFound }
    };

    public static int Map(Error error)
    {
        if (_map.TryGetValue(error.Code, out var statusCode))
        {
            return statusCode;
        }

        return StatusCodes.Status400BadRequest;
    }
}
