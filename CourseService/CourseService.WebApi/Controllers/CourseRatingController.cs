using CourseService.Application.DTO;
using CourseService.Application.Interfaces.Services;
using CourseService.Domain.Results;
using CourseService.WebApi.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.WebApi.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId:guid}/ratings")]
    public class CourseRatingController : ControllerBase
    {
        private readonly ICourseRatingService _ratingService;

        public CourseRatingController(ICourseRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        /// <summary>
        /// Add a rating to a course.
        /// </summary>
        /// <remarks>
        /// User can rate a course with a value from 1 to 5.
        /// Each user can have only one rating per course.
        /// </remarks>
        /// <param name="courseId">Course identifier.</param>
        //[Authorize]
        [HttpPost]
        public async Task<Result<CourseRatingResponse>> CreateRating(CourseRatingDTO ratingDTO, Guid courseId)//[FromBody]
        {
            var userId = User.GetUserId();
            var result = await _ratingService.CreateCourseRatingAsync(ratingDTO, courseId, userId);
            return result;
        }

        /// <summary>
        /// Update course rating.
        /// </summary>
        /// <remarks>
        /// Available only to the user who created the rating.
        /// </remarks>
        /// <param name="courseId">Course identifier.</param>
        //[Authorize]
        [HttpPut]
        public async Task<Result<CourseRatingResponse>> UpdateRating(CourseRatingDTO ratingDTO, Guid courseId)
        {
            var userId = User.GetUserId();
            var result = await _ratingService.UpdateCourseRatingAsync(ratingDTO, courseId, userId);
            return result;
        }
    }
}
