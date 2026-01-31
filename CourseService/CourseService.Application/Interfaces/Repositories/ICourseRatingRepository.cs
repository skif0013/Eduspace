using CourseService.Domain.Entities;

namespace CourseService.Application.Interfaces.Repositories;

public interface ICourseRatingRepository
{
    Task CreateRatingAsync(CourseRating rating);
    Task<CourseRating> GetRatingByCourseIdAndUserIdAsync(Guid courseId, Guid userId);
    Task<List<CourseRating>> GetRatingsByCourseIdAsync(Guid courseId);
    Task UpdateRatingAsync(CourseRating newRating);
}
