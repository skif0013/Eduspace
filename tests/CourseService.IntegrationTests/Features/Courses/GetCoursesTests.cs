using CourseService.Application.Courses.DTO;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using CourseService.IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;
using CourseService.Infrastructure.Data;
using FluentAssertions;
using System.Net;
using CourseService.IntegrationTests.Common.Helpers;
using System.Net.Http.Json;
using CourseService.IntegrationTests.Common.Fixtures;

namespace CourseService.IntegrationTests.Features.Courses;

[Collection("Postgres collection")]
public class GetCoursesTests
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public GetCoursesTests(PostgresContainerFixture postgres)
    {
        _factory = new TestWebApplicationFactory(postgres);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task WhenCoursesExist_ShouldReturnPagedPublishedCourses()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        var courseId = Guid.NewGuid();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Courses.RemoveRange(db.Courses);


            db.Courses.Add(new Course
            {
                Id = courseId,
                AuthorId = Guid.NewGuid(),
                Name = "Test Course",
                Description = "Test Description",
                Status = CourseStatus.Published,
                Price = 0,
                IsFree = true
            });

            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/courses?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<PagedCoursesResponse>(TestJsonOptions.Default);

        result.Should().NotBeNull();
        result!.Courses.Should().NotBeEmpty();

        var course = result.Courses.FirstOrDefault(c => c.Id == courseId);

        course.Should().NotBeNull();
        course!.Name.Should().Be("Test Course");

        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task WhenNoCoursesExist_ShouldReturnEmptyList()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Courses.RemoveRange(db.Courses);
            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/courses?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<PagedCoursesResponse>(TestJsonOptions.Default);

        result.Should().NotBeNull();
        result!.Courses.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task WhenCoursesWithDifferentStatusesExist_ShouldReturnOnlyPublishedCourses()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Courses.RemoveRange(db.Courses);

            db.Courses.AddRange(
                new Course
                {
                    Id = Guid.NewGuid(),
                    AuthorId = Guid.NewGuid(),
                    Name = "Draft Course",
                    Description = "Test",
                    Status = CourseStatus.Draft
                },
                new Course
                {
                    Id = Guid.NewGuid(),
                    AuthorId = Guid.NewGuid(),
                    Name = "Archived Course",
                    Description = "Test",
                    Status = CourseStatus.Archived
                },
                new Course
                {
                    Id = Guid.NewGuid(),
                    AuthorId = Guid.NewGuid(),
                    Name = "Published Course",
                    Description = "Test",
                    Status = CourseStatus.Published
                }
            );

            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/courses?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<PagedCoursesResponse>(TestJsonOptions.Default);

        result.Should().NotBeNull();
        result!.Courses.Should().HaveCount(1);
        result.Courses.First().Name.Should().Be("Published Course");
    }

    [Fact]
    public async Task WhenPageAndPageSizeProvided_ShouldReturnExpectedPage()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        using (var arrangeScope = _factory.Services.CreateScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Courses.RemoveRange(db.Courses);

            var courses = Enumerable.Range(1, 25).Select(i => new Course
            {
                Id = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Name = $"Course {i}",
                Description = "Test",
                Status = CourseStatus.Published
            });

            db.Courses.AddRange(courses);
            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/courses?page=2&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<PagedCoursesResponse>(TestJsonOptions.Default);

        result.Should().NotBeNull();

        result!.Courses.Should().HaveCount(10);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
    }
}
