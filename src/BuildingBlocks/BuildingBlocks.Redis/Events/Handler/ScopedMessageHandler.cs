using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Redis.Events.Handler;

public abstract class ScopedMessageHandler<TEvent> where TEvent : class
{
    private readonly IServiceScopeFactory _scopeFactory;

    protected ScopedMessageHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public abstract string Channel { get; }
    
    // Сюда фоновый воркер передает сырую строку из Redis
    public async Task HandleMessageAsync(string message)
    {
        // Базовый класс один раз берет на себя рутину сериализации
        var @event = JsonSerializer.Deserialize<TEvent>(message);
        if (@event == null) return;

        using var scope = _scopeFactory.CreateScope();
        
        // Передаем в наследник уже готовый объект
        await HandleMessageAsync(@event, scope.ServiceProvider);
    }
    
    protected abstract Task HandleMessageAsync(TEvent @event, IServiceProvider serviceProvider);
}