using CourseService.Application.DTO;
using CourseService.Domain.Entities;
using CourseService.Domain.Results;

namespace CourseService.Application.Interfaces.Services;

public interface ICourseService
{
    Task<Result<bool>> ArchiveCourseAsync(Guid courseId, Guid authorId);
    Task<Result<CourseResponse>> CreateCourseAsync(CourseDTO courseDTO, Guid authorId);
    Task<Result<List<CourseResponse>>> GetAllCoursesAsync();
    Task<Result<CourseResponse>> GetCourseByIdAsync(Guid courseId);
    Task<Result<bool>> PublishCourseAsync(Guid courseId, Guid authorId);
    Task<Result<CourseResponse>> UpdateCourseAsync(CourseDTO courseDTO, Guid authorId, Guid courseId);
}
