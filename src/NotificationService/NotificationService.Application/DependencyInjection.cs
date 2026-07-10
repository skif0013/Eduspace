using BuildingBlocks.Redis.Events.Handler;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Redis.EventHadlers;

namespace NotificationService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IScopedMessageHandler, ConfirmEmailHandler>();
        services.AddSingleton<IScopedMessageHandler, ResetUserPasswordHandler>();

        return services;
    }
}
