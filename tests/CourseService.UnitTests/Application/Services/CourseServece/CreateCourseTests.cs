using AutoMapper;
using CourseService.Application.Caching;
using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Messaging;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CourseService.UnitTests.Application.Services.CourseServece;

public class CreateCourseTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ICourseCache> _cacheMock = new();
    private readonly Mock<ILogger<CourseService.Application.Courses.Services.CourseService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessagePublisher> _publisherMock = new();
    private readonly Mock<IRedisKeyBuilder> _keyBuilderMock = new();

    private readonly CourseService.Application.Courses.Services.CourseService _courseService; // SUT

    public CreateCourseTests()
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
    public async Task CreateCourseAsync_ShouldCreareCourse_WhenUserIsCourseAndLessonAuthorAndDataIsValid()
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
}
