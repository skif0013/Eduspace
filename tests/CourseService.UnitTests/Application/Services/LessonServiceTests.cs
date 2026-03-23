using AutoMapper;
using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Courses.Services;
using CourseService.Application.Messaging;
using CourseService.Domain.Abstractions;
using CourseService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
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
    //private readonly CourseService _courseService;

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

//        _courseService = new CourseService(
//            _courseRepositoryMock.Object,
//            _cacheMock.Object,
//            _lessonRepositoryMock.Object,
//            _loggerMock.Object,
//            _mapperMock.Object,
//            _publisherMock.Object,
//            _keyBuilderMock.Object
//);

    }

    // Lesson validation
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

    // Happy path
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

    // Course validations
    [Fact]
    public async Task DeleteLessonAsync_ShouldReturnFailure_WhenCourseNotFound()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        _courseRepositoryMock
            .Setup(x => x.GetCourseByIdAsync(courseId))
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
    //[Fact]
    //public async Task DeleteLessonAsync_ShouldReturnSuccess_WhenLessonDeleted()
    //{
    //    // Arrange
    //    var lessonId = Guid.NewGuid();
    //    var courseId = Guid.NewGuid();
    //    var authorId = Guid.NewGuid();
    //    var cacheKey = $"{_prefix}:course:{courseId}";

    //    var lesson = new Lesson
    //    {
    //        Id = lessonId,
    //        LessonNumber = 1,
    //        Name = "Test Lesson",
    //        Description = "Test Description",
    //        VideoUrl = "test-url",
    //        CreatedAt = DateTime.UtcNow,
    //    };

    //    _lessonRepositoryMock
    //        .Setup(x => x.DeleteLessonAsync(lesson));

    //    _keyBuilderMock
    //        .Setup(x => x.GetCourseKey(courseId))
    //        .ReturnsAsync(cacheKey);

    //    // Act 
    //    var result = await _lessonService.DeleteLessonAsync(lesson);
    //}

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
