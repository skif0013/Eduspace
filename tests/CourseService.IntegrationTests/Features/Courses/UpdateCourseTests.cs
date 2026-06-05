using CourseService.Application.Courses.DTO;
using CourseService.Infrastructure.Data;
using CourseService.IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using CourseService.IntegrationTests.Common.Fixtures;
using CourseService.IntegrationTests.Common.Helpers;

namespace CourseService.IntegrationTests.Features.Courses;

[Collection("Postgres collection")]
public class UpdateCourseTests
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public UpdateCourseTests(PostgresContainerFixture postgres)
    {
        _factory = new TestWebApplicationFactory(postgres);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ShouldUpdateCourse_WhenRequestIsValid()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                Name = "Old Name",
                Description = "Old Description",
                Price = 10,
                IsFree = false,
                AuthorId = authorId,
                Status = CourseStatus.Published
            };

            db.Courses.Add(course);
            await db.SaveChangesAsync();
        }

        var dto = new CourseDTO
        {
            Name = "Updated Course",
            Description = "Updated Description",
            Price = 0,
            IsFree = true,
            AvatarURL = "http://test.com"
        };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}", authorId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using (var assertScope = _factory.Services.CreateScope())
        {
            var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var updated = await db.Courses.FindAsync(courseId);

            updated!.Name.Should().Be(dto.Name);
            updated.Id.Should().Be(courseId);
        }
    }

    [Fact]
    public async Task ShouldReturnUnautorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var dto = new CourseDTO { Name = "Test" };
        var courseId = Guid.NewGuid();

        var request = HttpRequestFactory.CreateUnauthorized(HttpMethod.Put, $"/api/courses/{courseId}", dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ShouldReturnForbidden_WhenNotCourseAuthor()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var realAuthorId = Guid.NewGuid();
        var anoterAuthorId = Guid.NewGuid();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var course = new Course
            {
                Id = courseId,
                AuthorId = realAuthorId,
                Name = "Test Course",
                Description = "Test Description",
                Price = 0,
                IsFree = true,
                AvatarURL = "http://test.com",
                Status = CourseStatus.Published
            };

            db.Courses.Add(course);
            db.SaveChanges();
        }

        var dto = new CourseDTO
        {
            Name = "Updated Course",
            Description = "Updated Description",
            Price = 10,
            IsFree = false,
            AvatarURL = "http://updatedtest.com"
        };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}", anoterAuthorId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new CourseDTO
        {
            Name = "Test Course",
            Description = "Test Description",
            Price = 0,
            IsFree = true,
            AvatarURL = "http://test.com"
        };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SholdReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        var dto = new CourseDTO
        {
            Name = "",
            Description = "",
            Price = -10,
            IsFree = true,
            AvatarURL = "http://test.com"
        };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}", authorId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnValidationErrors_WhenRequestIsInvalid()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var dto = new CourseDTO
        {
            Name = "",
            Description = "Test",
            Price = -10,
            IsFree = true,
            AvatarURL = "invalid"
        };

        var request = HttpRequestFactory.CreateAuthorized(HttpMethod.Put, $"/api/courses/{courseId}", userId, dto);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        problem.Should().NotBeNull();

        problem.Errors.Should().ContainKey("Name");
        problem.Errors["Name"].Should().NotBeEmpty();
        problem.Errors.Should().ContainKey("Price");
    }
}
