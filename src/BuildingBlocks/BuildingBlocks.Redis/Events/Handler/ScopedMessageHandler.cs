using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Redis.Events.Handler;

public abstract class ScopedMessageHandler<TEvent> : IScopedMessageHandler where TEvent : class
{
    private readonly IServiceScopeFactory _scopeFactory;

    protected ScopedMessageHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public abstract string Channel { get; }

    public async Task HandleMessageAsync(string message)
    {
        var @event = JsonSerializer.Deserialize<TEvent>(message);
        if (@event == null) return;

        using var scope = _scopeFactory.CreateScope();
        
        await HandleMessageAsync(@event, scope.ServiceProvider);
    }
    
    protected abstract Task HandleMessageAsync(TEvent @event, IServiceProvider serviceProvider);
}