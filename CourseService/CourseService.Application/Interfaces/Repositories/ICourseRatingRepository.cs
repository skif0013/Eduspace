using CourseService.Domain.Entities;

namespace CourseService.Application.Interfaces.Repositories;

public interface ICourseRatingRepository
{
    Task<CourseRating> GetRatingByCourseIdAdnUserIdAsync(Guid courseId, Guid userId);
    Task CreateRatingAsync(CourseRating rating);
    Task UpdateRatingAsync(CourseRating newRating);
}
