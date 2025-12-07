namespace NotificationService.Application.Interfaces.Services;

public interface IMessageService
{
    Task SendMessageAsync<T>(string action, T message);
}