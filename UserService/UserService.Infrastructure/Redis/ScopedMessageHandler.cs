using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Interfaces.Services;

namespace UserService.Infrastructure.Redis;

public abstract class ScopedMessageHandler : IMessageHandler
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    protected ScopedMessageHandler(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public abstract string Channel { get; }

    public async Task HandleAsync(string message)
    {
        using var scope = _scopeFactory.CreateScope();
        
        await HandleScopedAsync(message, scope.ServiceProvider);
        
    }

    protected abstract Task HandleScopedAsync(string message, IServiceProvider serviceProvider);
}