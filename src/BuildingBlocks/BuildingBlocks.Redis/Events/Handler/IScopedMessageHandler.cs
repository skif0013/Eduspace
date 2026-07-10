namespace BuildingBlocks.Redis.Events.Handler;

public interface IScopedMessageHandler
{
    string Channel { get; }

    Task HandleMessageAsync(string message);
}
