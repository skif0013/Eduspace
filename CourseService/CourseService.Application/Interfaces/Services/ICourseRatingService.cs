using CourseService.Application.DTO;
using CourseService.Domain.Results;

namespace CourseService.Application.Interfaces.Services;

public interface ICourseRatingService
{
    Task<Result<bool>> CreateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId);
    Task<Result<bool>> UpdateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId);
}
