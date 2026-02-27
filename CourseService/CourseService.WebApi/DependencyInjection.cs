using CourseService.WebApi.Infrastructure.ExceptionHandling;

namespace CourseService.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApiServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();

        return app;
    }
}
