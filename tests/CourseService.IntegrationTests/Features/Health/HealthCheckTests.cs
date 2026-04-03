using CourseService.IntegrationTests.Common;
using FluentAssertions;
using System.Net;

namespace CourseService.IntegrationTests.Features.Health;

public class HealthCheckTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_UnkhownEndpoint_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/some-random-url");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
