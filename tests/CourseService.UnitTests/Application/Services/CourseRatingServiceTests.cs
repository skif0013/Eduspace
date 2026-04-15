using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Courses.Services;
using CourseService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CourseService.UnitTests.Application.Services;

public class CourseRatingServiceTests
{
    private readonly Mock<ICourseRatingRepository> _ratingRepositoryMock = new();
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseRatingService>> _loggerMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly ICourseRatingService _courseRatingService; // SUT

    public CourseRatingServiceTests()
    {
        _courseRatingService = new CourseRatingService(
            _ratingRepositoryMock.Object,
            _courseRepositoryMock.Object,
            _cacheMock.Object,
            _loggerMock.Object,
            _keyBuilderMock.Object);
    }

    [Fact]
    public async Task CreateRatingAsync_ShouldReturnCourseNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var courseRatingDto = new CourseRatingDTO { Rating = 1 };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _courseRatingService.CreateRatingAsync(courseRatingDto, courseId, userId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _ratingRepositoryMock.Verify(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId), Times.Never());
        _ratingRepositoryMock.Verify(x => x.CreateRatingAsync(It.IsAny<CourseRating>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task CreateRatingAsync_ShouldReturnRatingAlreadyExists_WhenUserAlreadyRatedCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var courseRatingDto = new CourseRatingDTO
        {
            Rating = 1,
        };

        var course = new Course
        {
            Id = courseId,
            CourseRatings = new List<CourseRating>()
        };

        var existingRating = new CourseRating
        {
            CourseId = courseId,
            UserId = userId
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _ratingRepositoryMock
            .Setup(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId))
            .ReturnsAsync(existingRating);

        // Act
        var result = await _courseRatingService.CreateRatingAsync(courseRatingDto, courseId, userId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseRatingErrors.RatingAlreadyExists);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _ratingRepositoryMock.Verify(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId), Times.Once());

        _ratingRepositoryMock.Verify(x => x.CreateRatingAsync(It.IsAny<CourseRating>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task CreateRatingAsync_ShouldCreateRating_WhenUserHasNotRatedCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new CourseRatingDTO
        {
            Rating = 4
        };

        var course = new Course
        {
            Id = courseId,
        };

        var ratings = new List<CourseRating>
        {
            new CourseRating { CourseId = courseId, Rating = 3 },
            new CourseRating { CourseId = courseId, Rating = 5 },
            new CourseRating { CourseId = courseId, Rating = dto.Rating }
        };

        var courseKey = "test-key";

        const double expectedAverageRating = 4.0;
        const int expectedAmountRating = 3;

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _ratingRepositoryMock
            .Setup(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId))
            .ReturnsAsync((CourseRating?)null);

        _ratingRepositoryMock
            .Setup(x => x.GetRatingsByCourseIdAsync(courseId))
            .ReturnsAsync(ratings);

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(courseKey);

        // Act 
        var result = await _courseRatingService.CreateRatingAsync(dto, courseId, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.AverageRating.Should().Be(expectedAverageRating);
        result.Value.AmountRatings.Should().Be(expectedAmountRating);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _ratingRepositoryMock.Verify(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId), Times.Once());
        _ratingRepositoryMock.Verify(x => x.CreateRatingAsync(It.IsAny<CourseRating>()), Times.Once());
        _ratingRepositoryMock.Verify(x => x.GetRatingsByCourseIdAsync(courseId), Times.Once());
        _keyBuilderMock.Verify(x => x.GetCourseKey(courseId), Times.Once());
        _cacheMock.Verify(x => x.RemoveAsync(courseKey), Times.Once());
    }

    [Fact]
    public async Task UpdateRatingAsync_ShouldReturnRatingNotFound_WhenRatingDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var ratingDto = new CourseRatingDTO { Rating = 1 };

        _ratingRepositoryMock
            .Setup(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId))
            .ReturnsAsync((CourseRating?)null);

        // Act
        var result = await _courseRatingService.UpdateRatingAsync(ratingDto, courseId, userId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseRatingErrors.RatingNotFound);

        _ratingRepositoryMock.Verify(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId), Times.Once());

        _ratingRepositoryMock.Verify(x => x.UpdateRatingAsync(It.IsAny<CourseRating>()), Times.Never());
        _ratingRepositoryMock.Verify(x => x.GetRatingsByCourseIdAsync(It.IsAny<Guid>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task UpdateRatingAsync_ShouldUpdateRating_WhenRatingExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var ratingDto = new CourseRatingDTO { Rating = 5 };

        var rating = new CourseRating 
        { 
            CourseId = courseId,
            UserId = userId,
            Rating = 1 
        };

        var ratings = new List<CourseRating>
        {
            new CourseRating { Rating = 5 },
            new CourseRating { Rating = 3 }
        };

        const double expectedAverageRating = 4.0;
        const int expectedAmountRating = 2;

        var cacheKey = "test-key";

        _ratingRepositoryMock
            .Setup(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId))
            .ReturnsAsync(rating);

        _ratingRepositoryMock
            .Setup(x => x.GetRatingsByCourseIdAsync(courseId))
            .ReturnsAsync(ratings);

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        // Act
        var result = await _courseRatingService.UpdateRatingAsync(ratingDto, courseId, userId);

        result.IsSuccess.Should().BeTrue();
        
        result.Value.AverageRating.Should().Be(expectedAverageRating);
        result.Value.AmountRatings.Should().Be(expectedAmountRating);

        rating.Rating.Should().Be(5);

        _ratingRepositoryMock.Verify(x => x.GetRatingByCourseIdAndUserIdAsync(courseId, userId), Times.Once());
        _ratingRepositoryMock.Verify(x => x.UpdateRatingAsync(It.IsAny<CourseRating>()), Times.Once());
        _ratingRepositoryMock.Verify(x => x.GetRatingsByCourseIdAsync(courseId), Times.Once());
        _keyBuilderMock.Verify(x => x.GetCourseKey(courseId), Times.Once());
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once());
    }
}
