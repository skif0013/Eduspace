using CourseService.Domain.Entities;

namespace CourseService.Application.Interfaces.Repositories;

public interface ICourseRatingRepository
{
    Task CreateRatingAsync(CourseRating rating);
    Task<CourseRating> GetRatingByCourseIdAdnUserIdAsync(Guid courseId, Guid userId);
    Task UpdateRatingAsync(CourseRating newRating);
}
