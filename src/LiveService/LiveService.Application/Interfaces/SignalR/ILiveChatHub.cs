namespace LiveService.Application.Interfaces.SignalR;

public interface ILiveChatHub
{
    Task ReceiveUserJoined(string message);
    Task ReceiveUserLeft(string message);
    Task ReceiveMessage(string streamId, string message, string senderConnectionId);
}