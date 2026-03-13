using CourseService.Application.Courses.Interfaces;
using CourseService.Application.Courses.Mappings;
using CourseService.Application.Courses.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CourseService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CourseProfile).Assembly);
        services.AddAutoMapper(typeof(LessonProfile).Assembly);

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddCourseServices();

        return services;
    }

    private static IServiceCollection AddCourseServices(this IServiceCollection services)
    {
        services.AddScoped<ICourseService, Courses.Services.CourseService>();
        services.AddScoped<ICourseRatingService, CourseRatingService>();
        services.AddScoped<ILessonService, LessonService>();

        return services;
    }
}
