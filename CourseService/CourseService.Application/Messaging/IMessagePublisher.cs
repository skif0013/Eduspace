namespace CourseService.Application.Messaging;

public interface IMessagePublisher
{
    Task<bool> PublishAsync(string channel, string message);
}
