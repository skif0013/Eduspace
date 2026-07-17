using LiveService.Application.Interfaces.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace LiveService.WebApi.SignalR;

public class LiveChatHub : Hub<ILiveChatHub>
{
    // Метод вызова с клиента (входящий)
    public async Task UserJoinedStreamChat(string streamId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, streamId);
        
        // Вызываем событие на клиентах через интерфейс
        await Clients.Group(streamId).ReceiveUserJoined($"User {Context.ConnectionId} joined the chat.");
    }

    // Метод вызова с клиента (входящий)
    public async Task UserLeftStreamChat(string streamId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, streamId);
        
        await Clients.Group(streamId).ReceiveUserLeft($"User {Context.ConnectionId} left the chat.");
    }

    // Метод вызова с клиента (входящий)
    public async Task SendMessage(string streamId, string message)
    {
        // Вызываем событие на клиентах через интерфейс
        await Clients.Group(streamId).ReceiveMessage(streamId, message, Context.ConnectionId);
    }
}