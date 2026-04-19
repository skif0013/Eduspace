using AutoMapper;
using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Messaging;
using CourseService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CourseService.UnitTests.Application.Services.CourseServece;

public class GetPagedCoursesTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseService.Application.Courses.Services.CourseService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly CourseService.Application.Courses.Services.CourseService _courseService; // SUT

    public GetPagedCoursesTests()
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
    public async Task GetPagedCoursesAsync_ShouldReturnCachedCourses_WhenCacheHit()
    {
        // Arrange
        var page = 1;
        var pageSize = 4;

        var cacheVersion = 1;
        var cacheKey = "test-key";
        var cachedData = new PagedCoursesResponse
        {
            Courses = new List<CourseResponse>(),
            TotalCount = 1,
            Page = page,
            PageSize = pageSize,
            TotalPages = 1
        };

        _cacheMock
            .Setup(x => x.GetCatalogVersionAsync())
            .ReturnsAsync(cacheVersion);

        _keyBuilderMock
            .Setup(x => x.GetCoursesPageKey(cacheVersion, page, pageSize))
            .Returns(cacheKey);

        _cacheMock
            .Setup(x => x.GetAsync<PagedCoursesResponse>(cacheKey))
            .ReturnsAsync(cachedData);

        // Act
        var result = await _courseService.GetPagedCoursesAsync(page, pageSize);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cachedData);

        _cacheMock.Verify(x => x.GetCatalogVersionAsync(), Times.Once());
        _keyBuilderMock.Verify(x => x.GetCoursesPageKey(cacheVersion, page, pageSize), Times.Once());
        _cacheMock.Verify(x => x.GetAsync<PagedCoursesResponse>(cacheKey), Times.Once());

        _courseRepositoryMock.Verify(x => x.GetPagedCoursesAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        _cacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<PagedCoursesResponse>(), It.IsAny<CacheEntryType>()), Times.Never());
        _mapperMock.Verify(x => x.Map<CourseResponse>(It.IsAny<Course>()), Times.Never());
    }

    [Fact]
    public async Task GetPagedCoursesAsync_ShouldReturnCoursesFromRepository_WhenCacheMiss()
    {
        // Arrange
        var page = 1;
        var pageSize = 4;

        var cacheVersion = 1;
        var cacheKey = "test-key";

        var course = new Course
        {
            CourseRatings = new List<CourseRating>
            {
                new CourseRating { Rating = 1 },
                new CourseRating { Rating = 4 },
            }
        };

        var course2 = new Course
        {
            CourseRatings = new List<CourseRating>
            {
                new CourseRating { Rating = 4 },
                new CourseRating { Rating = 5 },
            }
        };

        var course3 = new Course
        {
            CourseRatings = new List<CourseRating>
            {
                new CourseRating { Rating = 5 },
                new CourseRating { Rating = 5 },
            }
        };

        var pagedCourses = new PagedResult<Course>
        {
            Items = new List<Course> { course, course2, course3 },
            TotalCount = 3
        };

        const double expectedAverage1 = 2.5;
        const double expectedAverage2 = 4.5;
        const double expectedAverage3 = 5.0;

        const int expectedRatingsCount = 2;

        var mappedCourses = new List<Course>();

        _cacheMock
            .Setup(x => x.GetCatalogVersionAsync())
            .ReturnsAsync(cacheVersion);

        _keyBuilderMock
            .Setup(x => x.GetCoursesPageKey(cacheVersion, page, pageSize))
            .Returns(cacheKey);

        _cacheMock
            .Setup(x => x.GetAsync<PagedCoursesResponse>(cacheKey))
            .ReturnsAsync((PagedCoursesResponse)null);

        _courseRepositoryMock
            .Setup(x => x.GetPagedCoursesAsync(page, pageSize))
            .ReturnsAsync(pagedCourses);

        _mapperMock
            .Setup(x => x.Map<CourseResponse>(It.IsAny<object>()))
            .Callback((object src) => mappedCourses.Add((Course)src))
            .Returns(() => new CourseResponse());

        // Act
        var result = await _courseService.GetPagedCoursesAsync(page, pageSize);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNull();
        result.Value.Page.Should().Be(page);
        result.Value.PageSize.Should().Be(pageSize);
        result.Value.TotalCount.Should().Be(3);
        result.Value.TotalPages.Should().Be(1);

        result.Value.Courses.Should().HaveCount(3);

        result.Value.Courses[0].AverageRating.Should().Be(expectedAverage1);
        result.Value.Courses[1].AverageRating.Should().Be(expectedAverage2);
        result.Value.Courses[2].AverageRating.Should().Be(expectedAverage3);

        result.Value.Courses[0].AmountRatings.Should().Be(expectedRatingsCount);
        result.Value.Courses[1].AmountRatings.Should().Be(expectedRatingsCount);
        result.Value.Courses[2].AmountRatings.Should().Be(expectedRatingsCount);

        mappedCourses.Should().HaveCount(3);
        mappedCourses.Should().BeEquivalentTo(pagedCourses.Items);

        _cacheMock.Verify(x => x.GetCatalogVersionAsync(), Times.Once());
        _keyBuilderMock.Verify(x => x.GetCoursesPageKey(cacheVersion, page, pageSize), Times.Once());
        _cacheMock.Verify(x => x.GetAsync<PagedCoursesResponse>(cacheKey), Times.Once());

        _courseRepositoryMock.Verify(x => x.GetPagedCoursesAsync(page, pageSize), Times.Once());

        _mapperMock.Verify(x => x.Map<CourseResponse>(It.IsAny<Course>()), Times.Exactly(3));

        _cacheMock.Verify(x => x.SetAsync(
            cacheKey,
            It.Is<PagedCoursesResponse>(r =>
                r.Page == page &&
                r.PageSize == pageSize &&
                r.TotalCount == 3 &&
                r.Courses.Count == 3),
            CacheEntryType.Catalog),
            Times.Once);
    }
}
