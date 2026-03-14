using CourseService.Domain.Abstractions;
using CourseService.WebApi.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.WebApi.Extentions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        var statusCode = HttpErrorMapper.Map(result.Error);

        return controller.Problem(
            title: result.Error.Code,
            detail: result.Error.Description,
            statusCode: statusCode);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        var statusCode = HttpErrorMapper.Map(result.Error);

        return controller.Problem(
            title: result.Error.Code,
            detail: result.Error.Description,
            statusCode: statusCode);
    }

    public static IActionResult ToCreatedActionResult<T>(this Result<T> result, ControllerBase controller, string location)
    {
        if (result.IsSuccess)
        {
            return controller.Created(location, result.Value);
        }

        var statusCode = HttpErrorMapper.Map(result.Error);

        return controller.Problem(
            title: result.Error.Code,
            detail: result.Error.Description,
            statusCode: statusCode);
    }
}
