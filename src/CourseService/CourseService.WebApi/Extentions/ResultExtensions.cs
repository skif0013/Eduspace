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

        var problem = new ProblemDetails
        {
            Title = result.Error.Code,
            Detail = result.Error.Description,
            Status = statusCode
        };

        return new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        var statusCode = HttpErrorMapper.Map(result.Error);

        if (statusCode == StatusCodes.Status400BadRequest)
        {
            var validation = new ValidationProblemDetails
            {
                Title = result.Error.Code,
                Detail = result.Error.Description,
                Status = statusCode
            };

            return new BadRequestObjectResult(validation);
        }

        var problem = new ProblemDetails
        {
            Title = result.Error.Code,
            Detail = result.Error.Description,
            Status = statusCode
        };

        return new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
    }

    public static IActionResult ToCreatedActionResult<T>(this Result<T> result, ControllerBase controller, string location)
    {
        if (!result.IsSuccess)
        {
            return result.ToActionResult(controller);
        }

        return controller.Created(location, result.Value);
    }
}
