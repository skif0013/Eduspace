using CourseService.Application.Caching;
using CourseService.Application.DTO;
using CourseService.Application.Extentions;
using CourseService.Application.Interfaces.Repositories;
using CourseService.Application.Interfaces.Services;
using CourseService.Domain.Entities;
using CourseService.Domain.Results;

namespace CourseService.Application.Services;

public class CourseRatingService : ICourseRatingService
{
    private readonly ICourseRatingRepository _ratingRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseCache _cache;

    public CourseRatingService(
        ICourseRatingRepository ratingRepository,
        ICourseRepository courseRepository,
        ICourseCache cache)
    {
        _ratingRepository = ratingRepository;
        _courseRepository = courseRepository;
        _cache = cache;
    }

    public async Task<Result<CourseRatingResponse>> CreateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId)
    {
        var course = await _courseRepository.GetCourseByIdAsync(courseId);
        if (course == null)
        {
            return Result<CourseRatingResponse>.Failure($"Course with {courseId} not found");
        }

        var exists = await _ratingRepository.GetRatingByCourseIdAndUserIdAsync(courseId, userId);   
        if(exists != null)
        {
            return Result<CourseRatingResponse>.Failure($"User with {userId} has already rated this course.");
        }

        var rating = new CourseRating
        {
            CourseId = courseId,
            UserId = userId,
            Rating = ratingDTO.Rating,
        };
        await _ratingRepository.CreateRatingAsync(rating);

        var (average, amount) = CourseRatingExtensions.CalculateRating(course.CourseRatings);

        var response = new CourseRatingResponse
        {
            AverageRating = average,
            AmountRatings = amount
        };

        await _cache.RemoveAsync($"course:{course.Id}"); 

        return Result<CourseRatingResponse>.Success(response);
    }

    public async Task<Result<CourseRatingResponse>> UpdateRatingAsync(CourseRatingDTO ratingDTO, Guid courseId, Guid userId)
    {
        var rating = await _ratingRepository.GetRatingByCourseIdAndUserIdAsync(courseId, userId);
        if (rating == null)
        {
            return Result<CourseRatingResponse>.Failure($"User with {userId} hasn`t already rated this course.");
        }

        rating.Rating = ratingDTO.Rating;
        await _ratingRepository.UpdateRatingAsync(rating);

        var ratings = await _ratingRepository.GetRatingsByCourseIdAsync(courseId);
        var (average, amount) = CourseRatingExtensions.CalculateRating(ratings);

        var response = new CourseRatingResponse
        {
            AverageRating = average,
            AmountRatings = amount
        };

        await _cache.RemoveAsync($"course:{courseId}");

        return Result<CourseRatingResponse>.Success(response);
    }
}
