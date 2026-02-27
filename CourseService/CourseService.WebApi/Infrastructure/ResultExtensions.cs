using CourseService.Domain.Abstractions;
using CourseService.WebApi.Infrastructure.Http;

namespace CourseService.WebApi.Infrastructure;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        var statusCode = HttpErrorMapper.Map(result.Error);

        return Results.Problem(
            title: result.Error.Code,
            detail: result.Error.Description,
            statusCode: statusCode);
    }
}
