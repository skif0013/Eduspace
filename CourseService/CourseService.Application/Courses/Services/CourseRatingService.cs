using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Extentions;
using CourseService.Domain.Abstractions;
using CourseService.Domain.Entities;

namespace CourseService.Application.Courses.Services;

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
            return Result<CourseRatingResponse>.Failure(CourseErrors.CourseNotFound);
        }
        
        var exists = await _ratingRepository.GetRatingByCourseIdAndUserIdAsync(courseId, userId);
        if (exists != null)
        {
            return Result<CourseRatingResponse>.Failure(CourseRatingErrors.RatingAlreadyExists);
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
            return Result<CourseRatingResponse>.Failure(CourseRatingErrors.RatingNotFound);
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
