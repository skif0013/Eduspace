using CourseService.Application.Courses.DTO;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using CourseService.IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json;
using CourseService.Infrastructure.Data;
using FluentAssertions;
using System.Net;

namespace CourseService.IntegrationTests.Features.Courses;

public class GetCoursesTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public GetCoursesTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task Get_Courses_WhenCoursesExist_ShouldReturnPagedPublishedCourses()
    {
        // Arrange
        var courseId = await SeedAsync();

        // Act
        var response = await _client.GetAsync("/api/courses?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<PagedCoursesResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });

        result.Should().NotBeNull();
        result!.Courses.Should().NotBeEmpty();

        var course = result.Courses.FirstOrDefault(c => c.Id == courseId);

        course.Should().NotBeNull();
        course!.Name.Should().Be("Test Course");

        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(1);
    }

    private async Task<Guid> SeedAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        var courseId = Guid.NewGuid();

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

        return courseId;
    }

    [Fact]
    public async Task Get_Courses_WhenNoCoursesExist_ShouldReturnEmptyList()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        // Act
        var response = await _client.GetAsync("/api/courses?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PagedCoursesResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });

        result.Should().NotBeNull();
        result!.Courses.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Get_Courses_WhenCoursesWithDifferentStatusesExist_ShouldReturnOnlyPublishedCourses()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

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

        // Act
        var response = await _client.GetAsync("/api/courses?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PagedCoursesResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });

        result.Should().NotBeNull();
        result!.Courses.Should().HaveCount(1);
        result.Courses.First().Name.Should().Be("Published Course");
    }

    [Fact]
    public async Task Get_Courses_WhenPageAndPageSizeProvided_ShouldReturnExpectedPage()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

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

        // Act
        var response = await _client.GetAsync("/api/courses?page=2&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PagedCoursesResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });

        result.Should().NotBeNull();

        result!.Courses.Should().HaveCount(10);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
    }
}
