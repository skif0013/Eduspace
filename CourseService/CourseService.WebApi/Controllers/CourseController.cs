using Microsoft.AspNetCore.Mvc;
using CourseService.WebApi.Extentions;
using Microsoft.AspNetCore.Authorization;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Interfaces;

namespace CourseService.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/courses")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;        
        }

        /// <summary>
        /// Get a paginated list of published courses.
        /// </summary>
        /// <param name="request">
        /// Pagination parameters (page number and page size).
        /// </param>
        /// <remarks>
        /// Returns only courses with status <b>Published</b>.
        /// Used for the public course catalog.
        /// Results are ordered by creation date (newest first).
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetPagedCourses([FromQuery] PaginationRequest request)   
        {
            var result = await _courseService.GetPagedCoursesAsync(request.Page, request.PageSize);

            return result.ToActionResult(HttpContext);
        }

        /// <summary>
        /// Get course by identifier.
        /// </summary>
        /// <remarks>
        /// Public access is allowed only for published courses.
        /// Course owner can access the course regardless of its status.
        /// </remarks>
        /// <param name="courseId">Course identifier.</param>
        [HttpGet("{courseId:guid}")]
        public async Task<IActionResult> GetCourseById(Guid courseId)
        {
            var result = await _courseService.GetCourseByIdAsync(courseId);

            return result.ToActionResult(HttpContext);
        }

        /// <summary>
        /// Create a new course.
        /// </summary>
        /// <remarks>
        /// Course is created with status <b>Draft</b>.
        /// Available only to the course author.
        /// </remarks>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCourse(CourseDTO courseDTO)
        {
            var authorId = User.GetUserId();
            var result = await _courseService.CreateCourseAsync(courseDTO, authorId);

            return result.ToActionResult(HttpContext);
        }

        /// <summary>
        /// Update an existing course.
        /// </summary>
        /// <remarks>
        /// Available only to the course owner.
        /// Published courses may have editing restrictions.
        /// </remarks>
        /// <param name="courseId">Course identifier.</param>
        /// <param name="courseDto">Updated course data.</param>
        [Authorize]
        [HttpPut("{courseId:guid}")]
        public async Task<IActionResult> UpdateCourse(CourseDTO courseDTO, Guid courseId)
        {
            var authorId = User.GetUserId();
            var result = await _courseService.UpdateCourseAsync(courseDTO, courseId, authorId);

            return result.ToActionResult(HttpContext);
        }

        /// <summary>
        /// Publish a course.
        /// </summary>
        /// <remarks>
        /// Changes course status from <b>Draft</b> to <b>Published</b>.
        /// After publishing, the course becomes visible in the public catalog.
        /// </remarks>
        /// <param name="courseId">Course identifier.</param>
        [Authorize]
        [HttpPatch("{courseId:guid}/publish")]
        public async Task<IActionResult> PublishCourse(Guid courseId)
        {
            var authorId = User.GetUserId();
            var result = await _courseService.PublishCourseAsync(courseId, authorId);

            return result.ToActionResult(HttpContext);
        }

        /// <summary>
        /// Archive a course.
        /// </summary>
        /// <remarks>
        /// Removes the course from the public catalog.
        /// The course becomes invisible.
        /// </remarks>
        /// <param name="courseId">Course identifier.</param>
        [Authorize]
        [HttpPatch("{courseId:guid}/archive")]
        public async Task<IActionResult> ArchiveCourse(Guid courseId)
        {
            var authorId = User.GetUserId();
            var result = await _courseService.ArchiveCourseAsync(courseId, authorId);

            return result.ToActionResult(HttpContext);
        }
    }
}