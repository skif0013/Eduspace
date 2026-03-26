using AutoMapper;
using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Messaging;
using CourseService.Domain.Abstractions;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
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

    //[Fact]
    //public async Task GetCourseByIdAsync_ReturnCourseResponse()
    //{
    //    var a = new { Name = "Test" };
    //    var b = new { Name = "Test" };

    //    a.Should().BeEquivalentTo(b);
    //}

    //[Theory]
    //[InlineData(1, 2, 4)]
    //[Fact]
    //public async Task GetLessonByIdAsync_ShouldReturnFaild()
    //{
    //    //result.Should().BeOfType<LessonResponse>();
    //    //result.Should().ContainEquivalentOf(expected);
    //    //result.Should().Contain(x => x.IsFree == true);
    //}

}
