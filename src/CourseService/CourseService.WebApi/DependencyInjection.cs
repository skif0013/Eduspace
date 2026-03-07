using CourseService.WebApi.Middlewares;

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
        app.UseMiddleware<CustomExceptionMiddleware>();

        return app;
    }
}
