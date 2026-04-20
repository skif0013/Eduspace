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
public class CreateLessonTests
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public CreateLessonTests(PostgresContainerFixture postgres)
    {
        _factory = new TestWebApplicationFactory(postgres);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ShouldCreateLesson_WhenRequesIsValid()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 1,
            Name = "Test Name",
            Description = "Test Description",
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

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Post, $"/api/courses/{courseId}/lessons", authorId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/courses/{courseId}/lessons/");

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

            var created = await db.Lessons.SingleOrDefaultAsync(x => x.Id == content.Id);
            
            created.Should().NotBeNull();
            created!.LessonNumber.Should().Be(dto.LessonNumber);
            created!.Name.Should().Be(dto.Name);
            created!.Description.Should().Be(dto.Description);
            created!.VideoUrl.Should().Be(dto.VideoUrl);
        }
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 1,
            Name = "Test Name",
            Description = "Test Description",
            VideoUrl = "http://test.com"
        };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Post, $"/api/courses/{courseId}/lessons", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldReturnForbidden_WhenUserIsNotCourseAuthor()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 1,
            Name = "Test Name",
            Description = "Test Description",
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

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Post, $"/api/courses/{courseId}/lessons", userId, dto);

        // Act 
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        content.Should().NotBeNull();
        content!.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Title.Should().Be(CourseErrors.NotCourseAuthor.Code);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenCourseIsArchived()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 1,
            Name = "Test Name",
            Description = "Test Description",
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

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Post, $"/api/courses/{courseId}/lessons", userId, dto);

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
    public async Task ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 0,
            Name = "",
            Description = "",
            VideoUrl = "http://test.com"
        };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Post, $"/api/courses/{courseId}/lessons", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        content.Should().NotBeNull();
        var errors = content!.Errors;

        errors.Should().ContainKey(nameof(LessonDTO.LessonNumber));
        errors.Should().ContainKey(nameof(LessonDTO.Name));
    }

    [Fact]
    public  async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();

        var dto = new LessonDTO
        {
            LessonNumber = 1,
            Name = "Test Name",
            Description = "Test Description",
            VideoUrl = "http://test.com"
        };

        var request = HttpRequestFactory.CreateUnauthorized(HttpMethod.Post, $"/api/courses/{courseId}/lessons", dto);

        // Act 
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
