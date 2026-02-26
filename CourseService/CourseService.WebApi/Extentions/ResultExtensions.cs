using CourseService.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.WebApi.Extentions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, HttpContext httpContext)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        var statusCode = HttpErrorMapper.Map(result.Error);

        var problem = new ProblemDetails
        {
            Title = result.Error.Code,
            Detail = result.Error.Description,
            Status = statusCode,
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        return new OkObjectResult(problem)
        {
            StatusCode = statusCode
        };
    }
}
