using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Interfaces;
using CourseService.Domain.Abstractions;
using CourseService.WebApi.Extentions;
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
        public async Task<IActionResult> CreateRating(CourseRatingDTO ratingDTO, Guid courseId)
        {
            var userId = User.GetUserId();
            var result = await _courseRatingService.CreateRatingAsync(ratingDTO, courseId, userId);
            
            return result.ToActionResult(HttpContext);
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
        public async Task<IActionResult> UpdateRating(CourseRatingDTO ratingDTO, Guid courseId)
        {
            var userId = User.GetUserId();
            var result = await _courseRatingService.UpdateRatingAsync(ratingDTO, courseId, userId);
            
            return result.ToActionResult(HttpContext);
        }
    }
}
