using CourseService.IntegrationTests.Common;
using CourseService.IntegrationTests.Common.Fixtures;
using FluentAssertions;
using System.Net;

namespace CourseService.IntegrationTests.Features.Health;

public class HealthCheckTests : IClassFixture<PostgresContainerFixture>
{
    private readonly HttpClient _client;

    public HealthCheckTests(PostgresContainerFixture postgres)
    {
        var factory = new TestWebApplicationFactory(postgres);
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
