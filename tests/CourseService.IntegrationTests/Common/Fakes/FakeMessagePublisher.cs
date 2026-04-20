using CourseService.Application.Messaging;

namespace CourseService.IntegrationTests.Common.Fakes;

public class FakeMessagePublisher : IMessagePublisher
{
    public Task PublishAsync(string channel, string message)
    {
        return Task.CompletedTask;
    }
}
