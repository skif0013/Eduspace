using CourseService.Application.Courses.DTO;
using CourseService.Domain.Entities;
using CourseService.Domain.Enums;
using CourseService.Infrastructure.Data;
using CourseService.IntegrationTests.Common;
using CourseService.IntegrationTests.Common.Fixtures;
using CourseService.IntegrationTests.Common.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace CourseService.IntegrationTests.Features.Lessons
{
    [Collection("Postgres collection")]
    public class GetLessonByIdTests
    {
        private readonly HttpClient _client;
        private readonly TestWebApplicationFactory _factory;

        public GetLessonByIdTests(PostgresContainerFixture postgres)
        {
            _factory = new TestWebApplicationFactory(postgres);
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task ShouldReturnLesson_WhenRequestIsValid()
        {
            // Arrange
            await _factory.ResetDatabaseAsync();

            var lessonId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            using (var assertScope = _factory.Services.CreateScope())
            {
                var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var course = new Course
                {
                    Id = courseId,
                    AuthorId = Guid.NewGuid(),
                    Name = "Test",
                    Description = "Test",
                    Price = 0,
                    IsFree = true,
                    AvatarURL = "http://test.com",
                    Status = CourseStatus.Published
                };

                db.Courses.Add(course);

                var lesson = new Lesson
                {
                    Id = lessonId,
                    CourseId = courseId,
                    Name = "Test Lesson",
                    Description = "Test Description",
                    VideoUrl = "http://test.com"
                };

                db.Lessons.Add(lesson);

                await db.SaveChangesAsync();
            }

            // Act
            var response = await _client.GetAsync($"/api/courses/{courseId}/lessons/{lessonId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<LessonResponse>();
            content.Should().NotBeNull();
            content.Id.Should().Be(lessonId);
            content.Name.Should().Be("Test Lesson");
        }

        [Fact]
        public async Task ShouldReturnNotFound_WhenLessonDoesNotExist()
        {
            // Arrange
            await _factory.ResetDatabaseAsync();

            var lessonId = Guid.NewGuid();
            var existingLessonId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            using (var arrengeScope = _factory.Services.CreateScope())
            {
                var db = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var course = new Course
                {
                    Id = courseId,
                    AuthorId = Guid.NewGuid(),
                    Name = "Test",
                    Description = "Test",
                    Price = 0,
                    IsFree = true,
                    AvatarURL = "http://test.com",
                    Status = CourseStatus.Published
                };

                db.Courses.Add(course);

                var lesson = new Lesson
                {
                    Id = anotherLessonId,
                    CourseId = courseId,
                    Name = "Test Lesson",
                    Description = "Test Description",
                    VideoUrl = "http://test.com"
                };

                db.Lessons.Add(lesson);

                await db.SaveChangesAsync();
            }

            // Act
            var response = await _client.GetAsync($"/api/courses/{courseId}/lessons/{lessonId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
            problem!.Status.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
