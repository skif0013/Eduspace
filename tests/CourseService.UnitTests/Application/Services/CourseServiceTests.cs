using AutoMapper;
using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Messaging;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CourseService.UnitTests.Application.Services;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseService.Application.Courses.Services.CourseService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly CourseService.Application.Courses.Services.CourseService _courseService;
    
    public CourseServiceTests()
    {
        // SUT
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
    public async Task ArchivedCourseAsync_ShouldReturnIsFailure_WhenCourseNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _courseService.ArchiveCourseAsync(courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task ArchivedCourseAsync_ShouldReturnIsFailure_WhenNotCourseAuthor()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var anotherAuthorId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            AuthorId = anotherAuthorId,
            Status = CourseStatus.Draft
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.ArchiveCourseAsync(courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotCourseAuthor);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task ArchivedCourseAsync_ShouldReturnSuccess_WhenCourseWasNotPublished()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Draft
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.ArchiveCourseAsync(courseId, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        course.Status.Should().Be(CourseStatus.Archived);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(course), Times.Once());

        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());

        _publisherMock.Verify(x => x.PublishAsync("course.archived",
            It.Is<string>(json =>
                json.Contains(course.Id.ToString()) &&
                json.Contains(course.AuthorId.ToString()))), 
            Times.Once());
    }

    [Fact]
    public async Task ArchivedCourseAsync_ShouldReturnSuccess_WhenCourseWasPublished()
    {
        // Arrage
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Published,
        };

        var cacheKey = "test-key";

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        // Act 
        var result = await _courseService.ArchiveCourseAsync(courseId, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        course.Status.Should().Be(CourseStatus.Archived);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(course), Times.Once());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Once());
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once());
        _publisherMock.Verify(x => x.PublishAsync("course.archived",
            It.Is<string>(json =>
                json.Contains(course.Id.ToString()) &&
                json.Contains(course.AuthorId.ToString()))
            ), Times.Once());
    }

    [Fact]
    public async Task CreateCourseAsync_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var courseDto = new CourseDTO
        {
            Name = "Test Name",
            Description = "Test Description",
            Price = 0,
            IsFree = true,
            AvatarURL = "Test url"
        };

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = courseDto.Name,
            Description = courseDto.Description,
            Price = courseDto.Price,
            IsFree = courseDto.IsFree,
            AvatarURL = courseDto.AvatarURL,
            Status = CourseStatus.Draft
        };

        var courseResponse = new CourseResponse
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            Price = course.Price,
            AvatarURL = course.AvatarURL,
            Status = course.Status,
        };

        _mapperMock
            .Setup(x => x.Map<Course>(courseDto))
            .Returns(course);

        _courseRepositoryMock
            .Setup(x => x.CreateCourseAsync(course))
            .ReturnsAsync(course);

        _mapperMock
            .Setup(x => x.Map<CourseResponse>(course))
            .Returns(courseResponse);

        // Act
        var result = await _courseService.CreateCourseAsync(courseDto, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(courseResponse);

        course.AuthorId.Should().Be(authorId);
        course.Status.Should().Be(CourseStatus.Draft);

        _mapperMock.Verify(x => x.Map<Course>(courseDto), Times.Once());
        _courseRepositoryMock.Verify(x => x.CreateCourseAsync(course), Times.Once());
        _mapperMock.Verify(x => x.Map<CourseResponse>(course), Times.Once());

        _publisherMock.Verify(x => x.PublishAsync(
            "course.created", 
            It.Is<string>(json =>
                json.Contains(course.Id.ToString()) &&
                json.Contains(authorId.ToString()))
            ), Times.Once());
    }

    [Fact]
    public async Task GetPagedCoursesAsync_ShouldReturnCachedData_AndNotCallDependencies()
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
    public async Task GetPagedCoursesAsync_ShouldReturnSuccess_AndData()
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
            Items = new List<Course> { course , course2, course3 },
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

    [Fact]
    public async Task GetCourseByIdAsync_ShouldReturnFailure_WhenCourseNotFound()
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
    public async Task GetCourseByIdAsync_ShouldReturnFailure_WhenCourseRequiresPayment()
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
    public async Task GetCourseByIdAsync_ShouldReturnSuccess_WhenDataCached()
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
    public async Task GetCourseByIdAsync_ShouldReturnSuccess_WhenValidData()
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

    [Fact]
    public async Task PublishCourseAsync_ShouldReturnFailure_WhenCourseNotFound()
    {
        // Arrage
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act 
        var result = await _courseService.PublishCourseAsync(courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task PublishCourseAsync_ShouldReturnFailure_WhenNotCourseAuthor()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var anotherAuthorId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            AuthorId = anotherAuthorId,
            Status = CourseStatus.Draft
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.PublishCourseAsync(courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotCourseAuthor);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task PublishCourseAsync_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Draft
        };

        var cacheKey = "test-key";

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        // Act
        var result = await _courseService.PublishCourseAsync(courseId, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        course.Status.Should().Be(CourseStatus.Published);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(course), Times.Once());   
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Once());
        _keyBuilderMock.Verify(x => x.GetCourseKey(courseId), Times.Once());
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once());

        _publisherMock.Verify(x => x.PublishAsync("course.published",
            It.Is<string>(json =>
                json.Contains(courseId.ToString()) &&
                json.Contains(authorId.ToString()))
            ), Times.Once());
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldReturnCourseNotFoundError_WhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var courseDto = new CourseDTO();

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act 
        var result = await _courseService.UpdateCourseAsync(courseDto, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _mapperMock.Verify(x => x.Map<CourseResponse>(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldReturnNotCourseAuthorError_WhenAuthorDoesNotOwnCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var courseDto = new CourseDTO
        {
            Name = "Ignored",
            Description = "Ignored",
            Price = 1
        };

        var anotherAuthorId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            AuthorId = anotherAuthorId,
            Status = CourseStatus.Published
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act 
        var result = await _courseService.UpdateCourseAsync(courseDto, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotCourseAuthor);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _mapperMock.Verify(x => x.Map<CourseResponse>(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }


    [Fact]
    public async Task UpdateCourseAsync_ShouldReturnCourseArchivedError_WhenCourseIsArchived()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var courseDto = new CourseDTO
        {
            Name = "Ignored",
            Description = "Ignored",
            Price = 1
        };

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Archived
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.UpdateCourseAsync(courseDto, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseArchived);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());

        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(It.IsAny<Course>()), Times.Never());
        _mapperMock.Verify(x => x.Map<CourseResponse>(It.IsAny<Course>()), Times.Never());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Never());
        _keyBuilderMock.Verify(x => x.GetCourseKey(It.IsAny<Guid>()), Times.Never());
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldUpdateCourseAndReturnSuccess_WhenAuthorIsOwnerAndCourseIsActive()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var courseDto = new CourseDTO
        {
            Name = "Updated-Name",
            Description = "Updated-Description",
            Price = 2,
            IsFree = true,
            AvatarURL = "Updated-url"
        };

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Name = "Name",
            Description = "Description",
            Price = 1,
            IsFree = false,
            AvatarURL = "url",
            CourseRatings = new List<CourseRating>
            {
                new CourseRating { Rating = 1 },
                new CourseRating { Rating = 5 }
            }
        };

        var cacheKey = "test-key";

        const double expectedAverage = 3.0;
        const int expectedRatingsCount = 2;

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _mapperMock
            .Setup(x => x.Map<CourseResponse>(It.IsAny<Course>()))
            .Returns(new CourseResponse());

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        // Act
        var result = await _courseService.UpdateCourseAsync(courseDto, courseId, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        course.Name.Should().BeEquivalentTo(courseDto.Name);
        course.Description.Should().BeEquivalentTo(courseDto.Description);
        course.Price.Should().Be(courseDto.Price);
        course.IsFree.Should().Be(courseDto.IsFree);
        course.AvatarURL.Should().Be(courseDto.AvatarURL);

        result.Value.AverageRating.Should().Be(expectedAverage);
        result.Value.AmountRatings.Should().Be(expectedRatingsCount);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _courseRepositoryMock.Verify(x => x.UpdateCourseAsync(course), Times.Once());
        _mapperMock.Verify(x => x.Map<CourseResponse>(course), Times.Once());
        _cacheMock.Verify(x => x.IncrementCatalogVersionAsync(), Times.Once());
        _keyBuilderMock.Verify(x => x.GetCourseKey(courseId), Times.Once());
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once());
    }
}
