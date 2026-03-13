using CourseService.Domain.Abstractions;
using CourseService.WebApi.Http;

namespace CourseService.WebApi.Extentions;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        var statusCode = HttpErrorMapper.Map(result.Error);

        return Results.Problem(
            title: result.Error.Code,
            detail: result.Error.Description,
            statusCode: statusCode);
    }

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

    public static IResult ToCreatedResult<T>(this Result<T> result, string location)
    {
        if (result.IsSuccess)
        {
            return Results.Created(location, result.Value);
        }

        var statusCode = HttpErrorMapper.Map(result.Error);

        return Results.Problem(
            title: result.Error.Code,
            detail: result.Error.Description,
            statusCode: statusCode);
    }
}
