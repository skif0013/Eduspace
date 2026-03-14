using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Interfaces;
using CourseService.WebApi.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/courses/{courseId:guid}/lessons")]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    /// <summary>
    /// Get lesson by identifier.
    /// </summary>
    /// <remarks>
    /// Returns lesson information for the specified course.
    /// </remarks>
    /// <param name="lessonId">Lesson identifier.</param>
    [AllowAnonymous]
    [HttpGet("{lessonId:guid}")]
    [ProducesResponseType(typeof(LessonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLessonById(Guid lessonId)
    {
        var result = await _lessonService.GetLessonByIdAsync(lessonId);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Create a new lesson.
    /// </summary>
    /// <remarks>
    /// Available only to the course author.
    /// </remarks>
    /// <param name="courseId">Course identifier.</param>
    /// <param name="lessonDTO">Lesson data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(LessonResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateLesson([FromBody] LessonDTO lessonDTO, Guid courseId)
    {
        var authorId = User.GetUserId();
        var result = await _lessonService.CreateLessonAsync(lessonDTO, courseId, authorId);

        return result.ToCreatedActionResult(this, $"/api/courses/{courseId}/lessons/{result.Value?.Id}");
    }

    /// <summary>
    /// Update an existing lesson.
    /// </summary>
    /// <remarks>
    /// Available only to the course owner.
    /// </remarks>
    /// <param name="lessonId">Lesson identifier.</param>
    /// <param name="lessonDto">Updated lesson data.</param>
    [HttpPut("{lessonId:guid}")]
    [ProducesResponseType(typeof(LessonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateLesson([FromBody] LessonDTO lessonDTO, Guid lessonId, Guid courseId)
    {
        var authorId = User.GetUserId();
        var result = await _lessonService.UpdateLessonAsync(lessonDTO, lessonId, courseId, authorId);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Delete a lesson.
    /// </summary>
    /// <remarks>
    /// Available only to the course author.
    /// </remarks>
    /// <param name="lessonId">Lesson identifier.</param>
    [HttpDelete("{lessonId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteLesson(Guid lessonId, Guid courseId)
    {
        var authorId = User.GetUserId();
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        return result.ToActionResult(this);
    }
}
