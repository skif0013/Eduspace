using CourseService.Application.DTO;
using CourseService.Domain.Entities;

namespace CourseService.Application.Interfaces.Repositories;

public interface ICourseRepository
{
    Task<Course> CreateCourseAsync(Course course);
    Task<Course> GetCourseByIdAsync(Guid courseId);
    Task<PagedResult<Course>> GetPagedCoursesAsync(int page, int pageSize);
    Task UpdateCourseAsync(Course course);
}
