using CourseService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using CourseService.Domain.Results;
using CourseService.Application.DTO;
using CourseService.Domain.Entities;

namespace CourseService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;        
        }

        [HttpGet]
        [Route("get-all")]
        public async Task<Result<List<Course>>> GetAllCourses()
        {
            var result = await _courseService.GetAllCoursesAsync();
            return result;
        }

        [HttpGet]
        [Route("{courseId}")]
        public async Task<Result<Course>> GetCourseById(Guid courseId)
        {
            var result = await _courseService.GetCourseByIdAsync(courseId);
            return result;
        }

        [HttpPost]
        [Route("create")]
        public async Task<Result<CourseResponse>> CreateCourse(CreateCourseDTO courseDTO, Guid ownerId)
        {
            var result = await _courseService.CreateCourseAsync(courseDTO, ownerId);
            return result;
        }

        [HttpPut]
        [Route("update")]
        public async Task<Result<CourseResponse>> UpdateCourse(UpdateCourseDTO courseDTO, Guid ownerId)
        {
            var result = await _courseService.UpdateCourseAsync(courseDTO, ownerId);
            return result;
        }
    }
}
