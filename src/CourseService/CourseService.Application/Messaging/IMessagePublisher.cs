namespace CourseService.Application.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync(string channel, string message);
}
