namespace IdentityService.Application.Interfaces.Services;

public interface IMessageHandler
{
    string Channel { get; }
    Task HandleAsync(string message);
}