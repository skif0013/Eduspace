using CourseService.Application.DTO;
using CourseService.Domain.Entities;
using CourseService.Domain.Results;

namespace CourseService.Application.Interfaces.Services;

internal interface ICourseService
{
    Task<Result<List<Course>>> GetAllCoursesAsync();
    Task<Result<Course>> GetCourseByIdAsync(Guid courseId);
    Task<Result<CourseResponse>> CreateCourseAsync(CreateCourseDTO courseDTO, Guid ownerId);
    Task<Result<CourseResponse>> UpdateCourseAsync(UpdateCourseDTO courseDTO, Guid ownerId);
}
