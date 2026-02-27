using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Interfaces;
using CourseService.WebApi.Extentions;
using CourseService.WebApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/courses/{courseId:guid}/ratings")]
    public class CourseRatingController : ControllerBase
    {
        private readonly ICourseRatingService _courseRatingService;

        public CourseRatingController(ICourseRatingService ratingService)
        {
            _courseRatingService = ratingService;
        }

        /// <summary>
        /// Add a rating to a course.
        /// </summary>
        /// <remarks>
        /// User can rate a course with a value from 1 to 5.
        /// Each user can have only one rating per course.
        /// </remarks>
        /// <param name="courseId">Course identifier.</param>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(CourseRatingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IResult> CreateRating(CourseRatingDTO ratingDTO, Guid courseId)
        {
            var userId = User.GetUserId();
            var result = await _courseRatingService.CreateRatingAsync(ratingDTO, courseId, userId);

            return result.ToHttpResult();
        }

        /// <summary>
        /// Update course rating.
        /// </summary>
        /// <remarks>
        /// Available only to the user who created the rating.
        /// </remarks>
        /// <param name="courseId">Course identifier.</param>
        [Authorize]
        [HttpPut]
        public async Task<IResult> UpdateRating(CourseRatingDTO ratingDTO, Guid courseId)
        {
            var userId = User.GetUserId();
            var result = await _courseRatingService.UpdateRatingAsync(ratingDTO, courseId, userId);

            return result.ToHttpResult();
        }
    }
}
