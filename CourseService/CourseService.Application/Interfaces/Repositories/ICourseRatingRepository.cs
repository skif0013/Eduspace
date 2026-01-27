using CourseService.Domain.Entities;

namespace CourseService.Application.Interfaces.Repositories;

public interface ICourseRatingRepository
{
    Task<Course> CreateRatingAsync(Course course);
    Task<Course> UpdateRatingAsync(Course course);
}
