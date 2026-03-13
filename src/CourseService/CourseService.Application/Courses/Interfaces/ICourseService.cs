using CourseService.Application.Courses.DTO;
using CourseService.Domain.Abstractions;

namespace CourseService.Application.Courses.Interfaces;

public interface ICourseService
{
    Task<Result> ArchiveCourseAsync(Guid courseId, Guid authorId);
    Task<Result<CourseResponse>> CreateCourseAsync(CourseDTO courseDTO, Guid authorId);
    Task<Result<PagedCoursesResponse>> GetPagedCoursesAsync(int page, int pageSize);
    Task<Result<CourseResponse>> GetCourseByIdAsync(Guid courseId);
    Task<Result> PublishCourseAsync(Guid courseId, Guid authorId);
    Task<Result<CourseResponse>> UpdateCourseAsync(CourseDTO courseDTO, Guid courseId, Guid authorId);
}
