using AutoMapper;
using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Events;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Courses.Services;
using CourseService.Application.Messaging;
using CourseService.Domain.Abstractions;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.AccessControl;
using Xunit;

namespace CourseService.UnitTests.Application.Services;

public class LessonServiceTests
{
    private readonly Mock<ILessonRepository> _lessonRepositoryMock = new();
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<LessonService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly LessonService _lessonService;

    public LessonServiceTests()
    {
        // SUT
        _lessonService = new LessonService(
            _courseRepositoryMock.Object,
            _cacheMock.Object,
            _lessonRepositoryMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _publisherMock.Object,
            _keyBuilderMock.Object
        );
    }

    [Fact]
    public async Task CreateLessonAsync_ShouldReturnFailure_WhenCourseNotFound()
    {
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var lessonDto = new LessonDTO();

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _lessonService.CreateLessonAsync(lessonDto, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once);

        _mapperMock.Verify(x => x.Map<Lesson>(It.IsAny<LessonDTO>), Times.Never);
        _lessonRepositoryMock.Verify(x => x.CreateLessonAsync(It.IsAny<Lesson>()), Times.Never);
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never);
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateLessonAsync_ShouldReturnFailure_WhenNotCourseAuthor()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var lessonDto = new LessonDTO();
        var course = new Course
        {
            Id = courseId,
            AuthorId = Guid.NewGuid(),
            Status = CourseStatus.Published
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _lessonService.CreateLessonAsync(lessonDto, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotCourseAuthor);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once);

        _mapperMock.Verify(x => x.Map<Lesson>(It.IsAny<LessonDTO>()), Times.Never);
        _lessonRepositoryMock.Verify(x => x.CreateLessonAsync(It.IsAny<Lesson>()), Times.Never);
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never);
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateLessonAsync_ShouldReturnFailure_WhenCourseArchived()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var lessonDto = new LessonDTO();
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
        var result = await _lessonService.CreateLessonAsync(lessonDto, courseId, authorId);

        // Assert

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseArchived);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once);

        _mapperMock.Verify(x => x.Map<Lesson>(It.IsAny<LessonDTO>()), Times.Never);
        _lessonRepositoryMock.Verify(x => x.CreateLessonAsync(It.IsAny<Lesson>()), Times.Never);
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never);
        _publisherMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateLessonAsync_ShouldReturnSuccess_WhenValidData()
    {
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var lessonDto = new LessonDTO();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Draft
        };

        var lesson = new Lesson
        {
            Id = Guid.NewGuid(),
        };

        var lessonResponse = new LessonResponse();

        string cacheKey = "test-key";

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _mapperMock
            .Setup(x => x.Map<Lesson>(lessonDto))
            .Returns(lesson);

        _lessonRepositoryMock
            .Setup(x => x.CreateLessonAsync(lesson))
            .ReturnsAsync(lesson);

        _mapperMock
            .Setup(x => x.Map<LessonResponse>(lesson))
            .Returns(lessonResponse);

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        // Act 
        var result = await _lessonService.CreateLessonAsync(lessonDto, courseId, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);
        result.Value.Should().BeEquivalentTo(lessonResponse);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once);

        _mapperMock.Verify(x => x.Map<Lesson>(lessonDto), Times.Once);
        _lessonRepositoryMock.Verify(x => x.CreateLessonAsync(lesson), Times.Once);
        _mapperMock.Verify(x => x.Map<LessonResponse>(lesson), Times.Once);

        _publisherMock.Verify(x => x.PublishAsync("lesson.created", It.IsAny<string>()), Times.Once);

        _keyBuilderMock.Verify(x => x.GetCourseKey(courseId), Times.Once);
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once);
    }

    [Fact]
    public async Task GetLessonByIdAsync_ShouldReturnFailure_WhenLessonNotFound()
    {
        // Arrange
        var lessonId = Guid.NewGuid();

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync((Lesson?)null);

        // Act
        var result = await _lessonService.GetLessonByIdAsync(lessonId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LessonErrors.LessonNotFound);

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once);
        _mapperMock.Verify(x => x.Map<LessonResponse>(It.IsAny<Lesson>()), Times.Never);
    }

    [Fact]
    public async Task GetLessonByIdAsync_ShouldReturnSuccess_WhenLessonExists()
    {
        // Arrange
        var lessonId = Guid.NewGuid();

        var lesson = new Lesson
        {
            Id = lessonId,
            LessonNumber = 1,
            Name = "Test Lesson",
            Description = "Test Description",
            VideoUrl = "test-url",
            CreatedAt = DateTime.UtcNow,
        };

        var expectedResponse = new LessonResponse
        {
            Id = lesson.Id,
            LessonNumber = lesson.LessonNumber,
            Name = lesson.Name,
            Description = lesson.Description,
            VideoUrl = lesson.VideoUrl,
            CreatedAt = lesson.CreatedAt,
        };

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        _mapperMock
            .Setup(x => x.Map<LessonResponse>(It.IsAny<Lesson>()))
            .Returns(expectedResponse);

        // Act
        var result = await _lessonService.GetLessonByIdAsync(lessonId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);
        result.Value.Should().BeEquivalentTo(expectedResponse);

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once);
        _mapperMock.Verify(x => x.Map<LessonResponse>(lesson), Times.Once);
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnFailure_WhenCourseNotFound()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId!))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once);

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(It.IsAny<Guid>()), Times.Never);
        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(It.IsAny<Lesson>()), Times.Never);
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnFailure_WhenNotCourseAuthor()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var course = new Course
        {
            Id = lessonId,
            AuthorId = Guid.NewGuid(),
            Status = CourseStatus.Published
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotCourseAuthor);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once);

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(It.IsAny<Guid>()), Times.Never);
        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(It.IsAny<Lesson>()), Times.Never);
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnFailure_WhenCourseArchived()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

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
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.CourseArchived);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once);

        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(It.IsAny<Guid>()), Times.Never);
        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(It.IsAny<Lesson>()), Times.Never);
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnFailure_WhenLessonNotFound()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Published
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync((Lesson?)null);

        // Act
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LessonErrors.LessonNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once);
        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once);

        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(It.IsAny<Lesson>()), Times.Never);
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnFailure_WhenLessonDoesNotBelongToCourse()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Published
        };

        var lesson = new Lesson
        {
            Id = lessonId,
            CourseId = Guid.NewGuid(),
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        // Act
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LessonErrors.LessonNotFound);

        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once);
        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once);

        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(It.IsAny<Lesson>()), Times.Never);
        _cacheMock.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnSuccess_WhenLessonDeleted()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var cacheKey = "test-key";

        var course = new Course
        {
            Id = courseId,
            AuthorId = authorId,
            Status = CourseStatus.Published
        };

        var lesson = new Lesson
        {
            Id = lessonId,
            CourseId = courseId,
            LessonNumber = 1,
            Name = "Test Lesson",
            Description = "Test Description",
            VideoUrl = "test-url",
            CreatedAt = DateTime.UtcNow,
        };

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _lessonRepositoryMock
            .Setup(x => x.GetLessonByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        _keyBuilderMock
            .Setup(x => x.GetCourseKey(courseId))
            .Returns(cacheKey);

        // Act 
        var result = await _lessonService.DeleteLessonAsync(lessonId, courseId, authorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);


        _courseRepositoryMock.Verify(x => x.GetCourseByIdAsync(courseId), Times.Once());
        _lessonRepositoryMock.Verify(x => x.GetLessonByIdAsync(lessonId), Times.Once());
        _lessonRepositoryMock.Verify(x => x.DeleteLessonAsync(lesson), Times.Once());
        _cacheMock.Verify(x => x.RemoveAsync(cacheKey), Times.Once());
    }

    ////[Theory]
    ////[InlineData(1, 2, 4)]
    //[Fact]
    //public async Task GetLessonByIdAsync_ShouldReturnFaild()
    //{
    //    // Arrange
    //    var lessonId = Guid.NewGuid();

    //    //_lessonRepoMock
    //    //    .Setup(x => x.GetLessonByIdAsync(lessonId)
    //    //    .ReturnsAsync(null));

    //    //_loggerMock
    //    //    .Setup(x => x.Map<LessonService>);
    //    //result.Should().BeOfType<LessonResponse>();
    //    //result.Should().ContainEquivalentOf(expected);
    //    //result.Should().Contain(x => x.IsFree == true);

    //}
}
