using CourseService.Domain.Entities;
using CourseService.Domain.Results;

namespace CourseService.Application.Interfaces.Repositories;

public interface ICourseRepository
{
    Task<Course> CreateCourseAsync(Course course);
    Task<Course> GetCourseByIdAsync(Guid courseId);
    Task<List<Course>> GetAllCoursesAsync();
    Task<Course> UpdateCourseAsync(Course course);
}
