using CourseService.Application.DTO;
using CourseService.Domain.Entities;
using CourseService.Domain.Results;

namespace CourseService.Application.Interfaces.Services;

public interface ICourseService
{
    Task<Result<bool>> ArchiveCourse(Guid courseId);
    Task<Result<CourseResponse>> CreateCourseAsync(CourseDTO courseDTO, Guid ownerId);
    Task<Result<List<CourseResponse>>> GetAllCoursesAsync();
    Task<Result<CourseResponse>> GetCourseByIdAsync(Guid courseId);
    Task<Result<bool>> PublishCourse(Guid courseId);
    Task<Result<CourseResponse>> UpdateCourseAsync(CourseDTO courseDTO, Guid ownerId, Guid courseId);
}
