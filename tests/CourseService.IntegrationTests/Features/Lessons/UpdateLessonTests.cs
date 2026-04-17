using CourseService.Application.Courses.DTO;
using CourseService.Application.Courses.Errors;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using CourseService.Infrastructure.Data;
using CourseService.IntegrationTests.Common;
using CourseService.IntegrationTests.Common.Fixtures;
using CourseService.IntegrationTests.Common.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace CourseService.IntegrationTests.Features.Lessons;

[Collection("Postgres collection")]
public class UpdateLessonTests
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public UpdateLessonTests(PostgresContainerFixture postgres)
    {
        _factory = new TestWebApplicationFactory(postgres);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ShouldUpdateLesson_WhenRequestIsValid()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 2,
            Name = "New Name",
            Description = "New Description",
            VideoUrl = "http://test.com"
        };

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                Name = "Test Name",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                AuthorId = userId,
                Status = CourseStatus.Published
            };

            db.Courses.Add(course);

            var lesson = new Lesson
            {
                Id = lessonId,
                CourseId = courseId,
                LessonNumber = 1,
                Name = "Old Name",
                Description = "Old Description",
                VideoUrl = "http://test.com"
            };

            db.Lessons.Add(lesson);

            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}/lessons/{lessonId}", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<LessonResponse>();
        
        content.Should().NotBeNull();
        content!.LessonNumber.Should().Be(dto.LessonNumber);
        content.Name.Should().Be(dto.Name);
        content.Description.Should().Be(dto.Description);
        content.VideoUrl.Should().Be(dto.VideoUrl);

        // Assert DB
        using (var assertScope = _factory.Services.CreateScope())
        {
            var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var updated = await db.Lessons.SingleOrDefaultAsync(x => x.Id == lessonId);
            
            updated.Should().NotBeNull();
            updated.LessonNumber.Should().Be(dto.LessonNumber);
            updated.Name.Should().Be(dto.Name);
            updated.Description.Should().Be(dto.Description);
            updated.VideoUrl.Should().Be(dto.VideoUrl);
        }
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 2,
            Name = "New Name",
            Description = "New Description",
            VideoUrl = "http://test.com"
        };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}/lessons/{lessonId}", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.NotFound);
        content.Title.Should().Be(CourseErrors.CourseNotFound.Code);
        content.Detail.Should().Be(CourseErrors.CourseNotFound.Description);
    }

    [Fact]
    public async Task ShouldReturnForbidden_WhenUserIsNotOwnerOfCourse()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var authorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 2,
            Name = "New Name",
            Description = "New Description",
            VideoUrl = "http://test.com"
        };

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                Name = "Test Name",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                AuthorId = authorId,
                Status = CourseStatus.Published
            };

            db.Courses.Add(course);

            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}/lessons/{lessonId}", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        content.Should().NotBeNull();
        content!.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Title.Should().Be(CourseErrors.NotCourseAuthor.Code);
        content.Detail.Should().Be(CourseErrors.NotCourseAuthor.Description);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenCourseIsArchived()
    {
        // Arrange 
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 2,
            Name = "New Name",
            Description = "New Description",
            VideoUrl = "http://test.com"
        };

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                Name = "Test Name",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                AuthorId = userId,
                Status = CourseStatus.Archived
            };

            db.Courses.Add(course);

            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}/lessons/{lessonId}", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        content.Should().NotBeNull();
        content!.Status.Should().Be((int)HttpStatusCode.Conflict);
        content.Title.Should().Be(CourseErrors.CourseArchived.Code);
        content.Detail.Should().Be(CourseErrors.CourseArchived.Description);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenLessonDoesNotExist()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 2,
            Name = "New Name",
            Description = "New Description",
            VideoUrl = "http://test.com"
        };

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                Name = "Test Name",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                AuthorId = userId,
                Status = CourseStatus.Published
            };

            db.Courses.Add(course);

            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}/lessons/{lessonId}", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        content.Should().NotBeNull();
        content!.Status.Should().Be((int)HttpStatusCode.NotFound);
        content.Title.Should().Be(LessonErrors.LessonNotFound.Code);
        content.Detail.Should().Be(LessonErrors.LessonNotFound.Description);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenLessonDoesNotBelongToCourse()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var anotherCourseId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 2,
            Name = "New Name",
            Description = "New Description",
            VideoUrl = "http://test.com"
        };

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                Name = "Test Name",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                AuthorId = userId,
                Status = CourseStatus.Published
            };

            var anotherCourse = new Course
            {
                Id = anotherCourseId,
                Name = "Test Name 2",
                Description = "Test Description 2",
                Price = 0,
                IsFree = true,
                AuthorId = userId,
                Status = CourseStatus.Published
            };

            db.Courses.AddRange(course, anotherCourse);

            var lesson = new Lesson
            {
                Id = lessonId,
                CourseId = anotherCourseId,
                LessonNumber = 1,
                Name = "Test Name",
                Description = "Test Description",
                VideoUrl = "http://test.com"
            };

            db.Lessons.Add(lesson);

            await db.SaveChangesAsync();
        }

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}/lessons/{lessonId}", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        content.Should().NotBeNull();
        content!.Status.Should().Be((int)HttpStatusCode.NotFound);
        content.Title.Should().Be(LessonErrors.LessonNotFound.Code);
        content.Detail.Should().Be(LessonErrors.LessonNotFound.Description);

        // Assert DB
        using (var assertScope = _factory.Services.CreateScope())
        {
            var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var lesson = await db.Lessons.SingleAsync(x => x.Id == lessonId);

            lesson.LessonNumber.Should().Be(1);
            lesson.Name.Should().Be("Test Name");
            lesson.Description.Should().Be("Test Description");
        }
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange 
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = -1,
            Name = "",
            Description = "",
            VideoUrl = "http://test.com"
        };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}/lessons/{lessonId}", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        content.Should().NotBeNull();
        content.Errors.Should().ContainKey(nameof(LessonDTO.LessonNumber));
        content.Errors.Should().ContainKey(nameof(LessonDTO.Name));
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsUnauthenticated()
    {
        // Arrange 
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 2,
            Name = "New Name",
            Description = "New Description",
            VideoUrl = "http://test.com"
        };

        var request = HttpRequestFactory.CreateUnauthorized(HttpMethod.Put, $"/api/courses/{courseId}/lessons/{lessonId}", dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
