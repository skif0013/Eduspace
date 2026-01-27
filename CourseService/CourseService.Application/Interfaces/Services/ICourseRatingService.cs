using CourseService.Application.DTO;
using CourseService.Domain.Results;

namespace CourseService.Application.Interfaces.Services;

public interface ICourseRatingService
{
    Task<Result<CourseRatingResponse>> CreateCourseRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid ownerId);
    Task<Result<CourseRatingResponse>> UpdateCourseRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid ownerId);
}
