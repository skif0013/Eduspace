using CourseService.Application.Courses.Errors;
using CourseService.Domain.Abstractions;

namespace CourseService.WebApi.Extentions;

public static class HttpErrorMapper
{
    private static readonly Dictionary<Error, int> _map = new()
    {
        { CourseErrors.CourseNotFound, StatusCodes.Status404NotFound },
        { CourseErrors.NotCourseAuthor, StatusCodes.Status403Forbidden },
        { CourseRatingErrors.RatingAlreadyExists, StatusCodes.Status409Conflict },
        { CourseRatingErrors.RatingNotFound, StatusCodes.Status404NotFound }
    };

    public static int Map(Error error)
        => _map.TryGetValue(error, out var status)
            ? status
            : StatusCodes.Status400BadRequest;

    //public static int Map(Error error)
    //{
    //    var res = _map.TryGetValue(error, out var status);

    //     return res ? status : StatusCodes.Status400BadRequest;
    //}
}
