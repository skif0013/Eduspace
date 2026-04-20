using AutoMapper;
using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Messaging;
using CourseService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CourseService.UnitTests.Application.Services.CourseServece;

public class GetCourseByIdTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseService.Application.Courses.Services.CourseService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly CourseService.Application.Courses.Services.CourseService _courseService; // SUT

    public GetCourseByIdTests()
    {
        _courseService = new CourseService.Application.Courses.Services.CourseService(
            _courseRepositoryMock.Object,
            _cacheMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _publisherMock.Object,
            _keyBuilderMock.Object
        );
    }

    [Fact]
    public async Task GetCourseByIdAsync_ShouldReturnCourseNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var cacheKey = "test-key";

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        _cacheMock
            .Setup(x => x.GetAsync<CourseResponse>(cacheKey))
            .ReturnsAsync((CourseResponse?)null);

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _courseService.GetCourseByIdAsync(courseId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _cacheMock.Verify(x => x.GetAsync<CourseResponse>(cacheKey), Times.Once());
        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _mapperMock.Verify(x => x.Map<CourseResponse>(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<CourseResponse>(), It.IsAny<CacheEntryType>()), Times.Never());
    }

    [Fact]
    public async Task GetCourseByIdAsync_ShouldReturnCourseRequiresPayment_WhenCourseIsNotFree()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var cacheKey = "test-key";

        var course = new Course
        {
            Id = courseId,
            IsFree = false
        };

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        _cacheMock
            .Setup(x => x.GetAsync<CourseResponse>(cacheKey))
            .ReturnsAsync((CourseResponse?)null);

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.GetCourseByIdAsync(courseId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseRequiresPayment);

        _cacheMock.Verify(x => x.GetAsync<CourseResponse>(cacheKey), Times.Once());
        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _mapperMock.Verify(x => x.Map<CourseResponse>(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<CourseResponse>(), It.IsAny<CacheEntryType>()), Times.Never());
    }

    [Fact]
    public async Task GetCourseByIdAsync_ShouldReturnCourseFromCache_WhenCacheHit()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var cacheKey = "test-key";

        var courseResponse = new CourseResponse
        {
            Id = courseId,
        };

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        _cacheMock
            .Setup(x => x.GetAsync<CourseResponse>(cacheKey))
            .ReturnsAsync(courseResponse);

        // Act 
        var result = await _courseService.GetCourseByIdAsync(courseId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(courseResponse);

        _keyBuilderMock.Verify(x => x.GetCourseKey(courseId), Times.Once());
        _cacheMock.Verify(x => x.GetAsync<CourseResponse>(cacheKey), Times.Once());

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(It.IsAny<Guid>()), Times.Never());
        _mapperMock.Verify(x => x.Map<CourseResponse>(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<CourseResponse>(), It.IsAny<CacheEntryType>()), Times.Never());
    }

    [Fact]
    public async Task GetCourseByIdAsync_ShouldReturnCourseFromRepository_WhenCacheMiss()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var cacheKey = "test-key";

        var course = new Course
        {
            Id = courseId,
            IsFree = true,
            CourseRatings = new List<CourseRating>
            {
                new CourseRating { Rating = 5 }
            }
        };

        var courseResponse = new CourseResponse
        {
            Id = courseId,
            AmountRatings = 1,
            AverageRating = 5
        };

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        _cacheMock
            .Setup(x => x.GetAsync<CourseResponse>(cacheKey))
            .ReturnsAsync((CourseResponse?)null);

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _mapperMock
            .Setup(x => x.Map<CourseResponse>(course))
            .Returns(courseResponse);

        _cacheMock
            .Setup(x => x.SetAsync(cacheKey, courseResponse, CacheEntryType.Course))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _courseService.GetCourseByIdAsync(courseId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().BeEquivalentTo(courseResponse);
        result.Value.AverageRating.Should().Be(5);
        result.Value.AmountRatings.Should().Be(1);

        _keyBuilderMock.Verify(x => x.GetCourseKey(courseId), Times.Once());
        _cacheMock.Verify(x => x.GetAsync<CourseResponse>(cacheKey), Times.Once());
        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _mapperMock.Verify(x => x.Map<CourseResponse>(course), Times.Once());
        _cacheMock.Verify(x => x.SetAsync(cacheKey, courseResponse, CacheEntryType.Course), Times.Once());
    }
}
